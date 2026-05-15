using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SoulKnight.Dungeon
{
    /// <summary>
    /// Generates a dungeon layout using Binary Space Partitioning (BSP).
    /// Rooms are carved out, connected by corridors, and populated via RoomManager.
    /// </summary>
    public class DungeonGenerator : MonoBehaviour
    {
        [Header("Dungeon Size")]
        [SerializeField] private int dungeonWidth = 80;
        [SerializeField] private int dungeonHeight = 60;
        [SerializeField] private int minRoomSize = 6;
        [SerializeField] private int maxRoomSize = 16;
        [SerializeField] private int maxSplitDepth = 5;

        [Header("Tilemaps")]
        [SerializeField] private Tilemap floorTilemap;
        [SerializeField] private Tilemap wallTilemap;
        [SerializeField] private TileBase floorTile;
        [SerializeField] private TileBase wallTile;

        [Header("Generation")]
        [SerializeField] private int seed = 0;
        [SerializeField] private bool useRandomSeed = true;

        private List<RectInt> rooms = new List<RectInt>();
        private BSPNode rootNode;

        public List<RectInt> Rooms => rooms;

        [ContextMenu("Generate Dungeon")]
        public void Generate()
        {
            if (useRandomSeed) seed = Random.Range(0, int.MaxValue);
            Random.InitState(seed);

            rooms.Clear();
            floorTilemap.ClearAllTiles();
            wallTilemap.ClearAllTiles();

            // BSP partition
            RectInt dungeonBounds = new RectInt(0, 0, dungeonWidth, dungeonHeight);
            rootNode = new BSPNode(dungeonBounds);
            rootNode.Split(minRoomSize, maxRoomSize, maxSplitDepth);

            // Collect rooms
            rootNode.GetLeafRooms(rooms);

            // Paint floors and walls
            foreach (var room in rooms)
                PaintRoom(room);

            PaintCorridors(rootNode);
            PaintWalls();

            Debug.Log($"[DungeonGenerator] Generated {rooms.Count} rooms (seed: {seed})");

            // Notify room manager
            GetComponent<RoomManager>()?.InitializeRooms(rooms);
        }

        private void PaintRoom(RectInt room)
        {
            // Shrink by 1 to leave space for walls
            for (int x = room.x + 1; x < room.xMax - 1; x++)
                for (int y = room.y + 1; y < room.yMax - 1; y++)
                    floorTilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
        }

        private void PaintCorridors(BSPNode node)
        {
            if (node == null || (node.Left == null && node.Right == null)) return;

            if (node.Left != null && node.Right != null)
            {
                Vector2Int a = node.Left.GetCenter();
                Vector2Int b = node.Right.GetCenter();
                PaintLShapedCorridor(a, b);
            }

            PaintCorridors(node.Left);
            PaintCorridors(node.Right);
        }

        private void PaintLShapedCorridor(Vector2Int a, Vector2Int b)
        {
            // Horizontal then vertical
            int x = a.x;
            while (x != b.x)
            {
                floorTilemap.SetTile(new Vector3Int(x, a.y, 0), floorTile);
                floorTilemap.SetTile(new Vector3Int(x, a.y + 1, 0), floorTile);
                x += (b.x > a.x) ? 1 : -1;
            }

            int y = a.y;
            while (y != b.y)
            {
                floorTilemap.SetTile(new Vector3Int(b.x, y, 0), floorTile);
                floorTilemap.SetTile(new Vector3Int(b.x + 1, y, 0), floorTile);
                y += (b.y > a.y) ? 1 : -1;
            }
        }

        private void PaintWalls()
        {
            BoundsInt bounds = floorTilemap.cellBounds;
            foreach (Vector3Int pos in bounds.allPositionsWithin)
            {
                if (floorTilemap.HasTile(pos)) continue;

                bool adjacentToFloor = false;
                for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                        if (floorTilemap.HasTile(pos + new Vector3Int(dx, dy, 0)))
                            adjacentToFloor = true;

                if (adjacentToFloor)
                    wallTilemap.SetTile(pos, wallTile);
            }
        }

        private void Start() => Generate();
    }

    // ─── BSP Node ─────────────────────────────────────────────────────────────

    public class BSPNode
    {
        public RectInt Bounds;
        public BSPNode Left, Right;
        private RectInt room;

        public BSPNode(RectInt bounds) { Bounds = bounds; }

        public void Split(int minRoom, int maxRoom, int depth)
        {
            if (depth <= 0 || !CanSplit(minRoom)) { CreateRoom(minRoom, maxRoom); return; }

            bool splitH = Random.value > 0.5f;
            if (Bounds.width > Bounds.height * 1.25f) splitH = false;
            else if (Bounds.height > Bounds.width * 1.25f) splitH = true;

            if (splitH)
            {
                int splitY = Random.Range(Bounds.y + minRoom, Bounds.yMax - minRoom);
                Left = new BSPNode(new RectInt(Bounds.x, Bounds.y, Bounds.width, splitY - Bounds.y));
                Right = new BSPNode(new RectInt(Bounds.x, splitY, Bounds.width, Bounds.yMax - splitY));
            }
            else
            {
                int splitX = Random.Range(Bounds.x + minRoom, Bounds.xMax - minRoom);
                Left = new BSPNode(new RectInt(Bounds.x, Bounds.y, splitX - Bounds.x, Bounds.height));
                Right = new BSPNode(new RectInt(splitX, Bounds.y, Bounds.xMax - splitX, Bounds.height));
            }

            Left.Split(minRoom, maxRoom, depth - 1);
            Right.Split(minRoom, maxRoom, depth - 1);
        }

        private bool CanSplit(int minRoom) => Bounds.width >= minRoom * 2 || Bounds.height >= minRoom * 2;

        private void CreateRoom(int minSize, int maxSize)
        {
            int w = Random.Range(minSize, Mathf.Min(maxSize, Bounds.width - 2));
            int h = Random.Range(minSize, Mathf.Min(maxSize, Bounds.height - 2));
            int x = Bounds.x + Random.Range(1, Bounds.width - w - 1);
            int y = Bounds.y + Random.Range(1, Bounds.height - h - 1);
            room = new RectInt(x, y, w, h);
        }

        public void GetLeafRooms(List<RectInt> list)
        {
            if (Left == null && Right == null) { list.Add(room); return; }
            Left?.GetLeafRooms(list);
            Right?.GetLeafRooms(list);
        }

        public Vector2Int GetCenter()
        {
            if (Left == null && Right == null)
                return new Vector2Int(room.x + room.width / 2, room.y + room.height / 2);
            Vector2Int lc = Left?.GetCenter() ?? Vector2Int.zero;
            Vector2Int rc = Right?.GetCenter() ?? Vector2Int.zero;
            return new Vector2Int((lc.x + rc.x) / 2, (lc.y + rc.y) / 2);
        }
    }
}
