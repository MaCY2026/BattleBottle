using System;
using System.Collections.Generic;
using UnityEngine;

// Scene の編集モードでも更新する。
[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class LiquidVolume : MonoBehaviour
{
    // 内部の点を Scene に表示するかどうか。
    public bool ShowPoints;
    // 水量の割合。0 は空、1 は満杯。
    [Range(0f, 1f)] public float Fill = 0.5f;
    // 最も長い辺の分割数。
    [Range(5, 80)] public int Resolution = 40;

    // 点のローカル座標の配列。
    [SerializeField, HideInInspector] Vector3[] localPoints;
    // 点の数。
    [SerializeField, HideInInspector] int pointCount;
    // メッシュを囲む範囲を保存する。
    [SerializeField, HideInInspector] Bounds bakedBounds;

    // 点のワールド高さの配列。
    double[] heights;
    // 低い順に並べた点のうち、水面に対応するインデックス。
    int surfaceRank = -1;

    // 水の割合（Shaderに値を渡すためのId）
    static readonly int FillHeightId = Shader.PropertyToID("_FillHeight");
    MeshRenderer waterRenderer;
    // 水位を渡すために使う。
    MaterialPropertyBlock properties;

    // コンポーネントのメニューから実行し、内部の点を事前に生成して保存する。
    [ContextMenu("Bake Volume Points")]
    void BakeVolumePoints()
    {
        // モデルのメッシュを取得する。
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        // モデルの頂点を取得する。
        Vector3[] vertices = mesh.vertices;

        // 点群を生成する、点と点の距離も計算する。
        localPoints = LiquidVolumeMath.Bake(
            vertices, mesh.triangles, Resolution, out double cellSize
        );

        // 点の数を記録する。
        pointCount = localPoints.Length;
        // 囲むBoxを記録する。
        bakedBounds = mesh.bounds;

        // 表示を更新する。
        UpdateSurface();
#if UNITY_EDITOR
        UnityEditor.SceneView.RepaintAll();
#endif
    }

    void OnEnable()
    {
        UpdateSurface();
    }

    // 点のデータとシェーダーを確認し、必要な作業用データを用意する。
    void PrepareData()
    {
        if (waterRenderer == null)
            waterRenderer = GetComponent<MeshRenderer>();

        if (properties == null)
            properties = new MaterialPropertyBlock();

        if (heights == null || heights.Length != localPoints.Length)
            heights = new double[localPoints.Length];
    }

    void Update()
    {
        // ExecuteAlways により、編集モードでもシーンに変更があると Update が呼ばれる。
        if (!Application.IsPlaying(gameObject)) UpdateSurface();
    }

    void LateUpdate()
    {
        // 再生中は、移動用スクリプトの Update が終わってから水位を更新する。
        if (Application.IsPlaying(gameObject)) UpdateSurface();
    }

    void UpdateSurface()
    {
        PrepareData();

        // 行列の Y 成分を座標変換して、heightsに追加する
        Matrix4x4 m = transform.localToWorldMatrix;
        for (int i = 0; i < localPoints.Length; i++)
        {
            Vector3 p = localPoints[i];
            heights[i] = (double)m.m10 * p.x + (double)m.m11 * p.y + (double)m.m12 * p.z;
        }

        // 例：点が5個あり、Fill = 0.5の場合
        // surfaceRank = Mathf.RoundToInt(0.5 × 4) = 2
        //
        // rank | heights[rank] | localPoints | 
        // -----|---------------|-------------|
        //   0  |       1       |  (2, 8, 4)  |
        //   1  |       2       |  (7, 3, 6)  |
        //   2  |       3       |  (8, 5, 3)  | ← surfaceRank
        //   3  |       4       |  (5, 6, 7)  |
        //
        // ※ 回転なし、スケール1の場合。高さはローカル座標のYと一致する。

        // 低い順に並べ替え、対応する点の座標も一緒に並べ替える
        Array.Sort(heights, localPoints);
        surfaceRank = Mathf.RoundToInt(Fill * (heights.Length - 1));
        double level = heights[surfaceRank];

        // LiquidShaderに設定
        waterRenderer.GetPropertyBlock(properties);
        properties.SetFloat(FillHeightId, (float)level);
        waterRenderer.SetPropertyBlock(properties);
    }

    // オブジェクトを選択した状態で、Scene ビューの再描画が必要になると、Unity が自動的に呼び出す。
    void OnDrawGizmosSelected()
    {
        // スイッチ?、Baked?、Bakedは成功？
        if (!ShowPoints || localPoints == null || localPoints.Length == 0) return;

        // 元の Gizmos の描画行列を保存する。
        Matrix4x4 previous = Gizmos.matrix;
        // 描画する点の変換行列が瓶のTransformにする。
        Gizmos.matrix = transform.localToWorldMatrix;
        // 描画する点の色
        Gizmos.color = Color.cyan;

        // 描画する点のサイズ
        float size = Mathf.Max(bakedBounds.size.x, Mathf.Max(bakedBounds.size.y, bakedBounds.size.z)) / Resolution * 0.1f;
        // 描画する点が最大約2000個になるように間引く。間隔は最低1にする。
        int stride = Mathf.Max(1, Mathf.CeilToInt(localPoints.Length / 2000f));

        // 点を描画する
        for (int i = 0; i < localPoints.Length; i += stride)
        {
            Gizmos.DrawSphere(localPoints[i], size);
        }

        // Gizmosを元に戻す
        Gizmos.matrix = previous;
    }

    // 泡の発生位置を取得する。
    public bool TryGetBubblePosition(out Vector3 worldPosition)
    {
        worldPosition = Vector3.zero;

        if (Fill <= 0f || localPoints == null || surfaceRank < 0)
            return false;

        // 水面以下の点をランダムに選ぶ
        int rank = UnityEngine.Random.Range(0, surfaceRank + 1);

        // localPointsはワールド座標のYが低い順に並べ替え済みなので、rankで直接点を選べる。
        worldPosition = transform.TransformPoint(localPoints[rank]);

        return true;
    }
}

// 点の生成と水位計算
public static class LiquidVolumeMath
{
    public static Vector3[] Bake(Vector3[] vertices, int[] triangles, int resolution, out double cellSize)
    {
        // 全頂点を調べ、メッシュを囲む範囲を求める。AABBと似てる。
        double minX = vertices[0].x;
        double minY = vertices[0].y;
        double minZ = vertices[0].z;
        double maxX = minX;
        double maxY = minY;
        double maxZ = minZ;

        foreach (Vector3 p in vertices)
        {
            minX = Math.Min(minX, p.x);
            minY = Math.Min(minY, p.y);
            minZ = Math.Min(minZ, p.z);
            maxX = Math.Max(maxX, p.x);
            maxY = Math.Max(maxY, p.y);
            maxZ = Math.Max(maxZ, p.z);
        }
        cellSize = Math.Max(maxX - minX, Math.Max(maxY - minY, maxZ - minZ)) / resolution;

        // 各方向に点の数を求める（サンプリング点の数はnx * ny * nz）
        int nx = (int)Math.Ceiling((maxX - minX) / cellSize);
        int ny = (int)Math.Ceiling((maxY - minY) / cellSize);
        int nz = (int)Math.Ceiling((maxZ - minZ) / cellSize);

        // 次のループ前の初期化
        var result = new List<Vector3>();
        var hits = new List<double>();
        double epsilon = cellSize * 0.000001;

        // サンプリング点の中から、瓶の内部にある点を抽出する。
        for (int ix = 0; ix < nx; ix++)
        {
            for (int iz = 0; iz < nz; iz++)
            {
                // x座標とz座標を計算。0.5は表面と重ならないようにするため（変更可能）。
                double x = minX + (ix + 0.5) * cellSize;
                double z = minZ + (iz + 0.5) * cellSize;

                // boxの範囲外なら無視する フィルターの役割
                if (x >= maxX || z >= maxZ) continue;

                hits.Clear();
                for (int t = 0; t < triangles.Length; t += 3)
                {
                    // 三角形の頂点の座標
                    Vector3 a = vertices[triangles[t]];
                    Vector3 b = vertices[triangles[t + 1]];
                    Vector3 c = vertices[triangles[t + 2]];

                    // 三角形をXZ平面に投影する。投影されたものを三角形ABCとする
                    // ベクトル AB = (ux, uz)
                    // ベクトル AC = (vx, vz)
                    double ux = (double)b.x - a.x;
                    double uz = (double)b.z - a.z;
                    double vx = (double)c.x - a.x;
                    double vz = (double)c.z - a.z;
                    // 面積 ＝ |determinant| ÷ 2
                    double determinant = ux * vz - uz * vx;

                    // XZ平面への投影面積がほぼ0の三角形を除外（万が一本当にすり抜けたら、下にif (unique % 2 != 0)でエラーが出る）
                    if (Math.Abs(determinant) < cellSize * cellSize * 1e-12) continue;

                    // XZ平面上で、頂点Aからサンプリング位置PへのベクトルAPを求める。
                    double dx = x - a.x;
                    double dz = z - a.z;
                    // AP = u × AB + v × AC を満たす係数u、vを求める。
                    // ↑↓ 連立二元一次方程式
                    // dx = u × ux + v × vx    ①
                    // dz = u × uz + v × vz    ②
                    double u = (dx * vz - dz * vx) / determinant;
                    double v = (ux * dz - uz * dx) / determinant;

                    // サンプリング位置PがABCの内部または辺上にある場合、
                    // 係数u、vから交点のY座標を求め、hitsに追加する。
                    if (u >= 0 && v >= 0 && u + v <= 1.0)
                    {
                        // y分量
                        // P' = A + u × AB + v × AC　
                        hits.Add(a.y + u * ((double)b.y - a.y) + v * ((double)c.y - a.y));
                    }
                }

                // ヒットしない時は無視
                if (hits.Count == 0) continue;

                // 低い順
                hits.Sort();
                RemoveDuplicateHits(hits, epsilon);

                // 重複を除いた交点の数を確認する。
                if (hits.Count % 2 != 0)
                {
                    Debug.LogError("交点の数が奇数になっている。");
                    continue;
                }

                // 交点を2つずつ取り出す。
                for (int i = 0; i < hits.Count; i += 2)
                {
                    double y1 = hits[i];
                    double y2 = hits[i + 1];

                    for (int iy = 0; iy < ny; iy++)
                    {
                        double y = minY + (iy + 0.5) * cellSize;

                        if (y > y1 && y < y2)
                        {
                            result.Add(new Vector3((float)x, (float)y, (float)z));
                        }
                    }
                }
            }
        }

        return result.ToArray();
    }

    // 元のリストから、重複する交点を取り除く。
    static void RemoveDuplicateHits(List<double> hits, double epsilon)
    {
        var uniqueHits = new List<double>();

        foreach (double height in hits)
        {
            if (uniqueHits.Count == 0)
            {
                uniqueHits.Add(height);
                continue;
            }

            double lastHeight = uniqueHits[uniqueHits.Count - 1];

            if (height - lastHeight > epsilon)
            {
                uniqueHits.Add(height);
            }
        }

        // 元のリストの内容を、去重後のデータに置き換える。
        hits.Clear();
        hits.AddRange(uniqueHits);
    }
}
