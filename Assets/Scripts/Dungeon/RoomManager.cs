using System.Collections.Generic;
using UnityEngine;

namespace SoulKnight.Dungeon
{
    /// <summary>
    /// Tracks room states (unexplored, active, cleared).
    /// Spawns enemies when the player enters, unlocks doors when cleared.
    /// </summary>
    public class RoomManager : MonoBehaviour
    {
        [Header("Room Settings")]
        [SerializeField] private GameObject enemySpawnMarkerPrefab;
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private int minEnemiesPerRoom = 2;
        [SerializeField] private int maxEnemiesPerRoom = 5;
        [SerializeField] private float spawnPadding = 1.5f;

        private List<RoomData> rooms = new List<RoomData>();
        private RoomData currentRoom;

        public void InitializeRooms(List<RectInt> roomRects)
        {
            rooms.Clear();
            foreach (var rect in roomRects)
            {
                rooms.Add(new RoomData
                {
                    Bounds = rect,
                    State = RoomState.Unexplored,
                    IsStartRoom = false,
                    IsBossRoom = false
                });
            }

            // Mark first room as start, last as boss
            if (rooms.Count > 0) rooms[0].IsStartRoom = true;
            if (rooms.Count > 1) rooms[rooms.Count - 1].IsBossRoom = true;

            Debug.Log($"[RoomManager] Initialized {rooms.Count} rooms.");
        }

        public void EnterRoom(RoomData room)
        {
            if (room.State == RoomState.Cleared || room.IsStartRoom) return;

            currentRoom = room;
            room.State = RoomState.Active;

            if (!room.IsBossRoom)
                SpawnEnemiesInRoom(room);
            else
                SpawnBoss(room);
        }

        private void SpawnEnemiesInRoom(RoomData room)
        {
            if (enemyPrefabs.Length == 0) return;

            int count = Random.Range(minEnemiesPerRoom, maxEnemiesPerRoom + 1);
            room.AliveEnemyCount = count;

            for (int i = 0; i < count; i++)
            {
                Vector2 spawnPos = GetRandomPositionInRoom(room.Bounds);
                GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

                if (enemy.TryGetComponent<Enemies.BaseEnemy>(out var baseEnemy))
                {
                    baseEnemy.OnEnemyDied.AddListener(() => OnEnemyDied(room));
                }
            }
        }

        private void SpawnBoss(RoomData room)
        {
            // TODO: spawn boss prefab at room center
            Vector2 center = new Vector2(room.Bounds.x + room.Bounds.width / 2f,
                                         room.Bounds.y + room.Bounds.height / 2f);
            Debug.Log($"Boss should spawn at {center}");
        }

        private void OnEnemyDied(RoomData room)
        {
            room.AliveEnemyCount--;
            if (room.AliveEnemyCount <= 0)
                ClearRoom(room);
        }

        private void ClearRoom(RoomData room)
        {
            room.State = RoomState.Cleared;
            Debug.Log("[RoomManager] Room cleared! Unlocking doors...");
            // TODO: unlock door objects in scene
        }

        private Vector2 GetRandomPositionInRoom(RectInt bounds)
        {
            float x = Random.Range(bounds.x + spawnPadding, bounds.xMax - spawnPadding);
            float y = Random.Range(bounds.y + spawnPadding, bounds.yMax - spawnPadding);
            return new Vector2(x, y);
        }

        public RoomData GetRoomAtPosition(Vector2 worldPos)
        {
            foreach (var room in rooms)
                if (room.Bounds.Contains(Vector2Int.FloorToInt(worldPos)))
                    return room;
            return null;
        }
    }

    // ─── Data Classes ──────────────────────────────────────────────────────────

    public class RoomData
    {
        public RectInt Bounds;
        public RoomState State;
        public bool IsStartRoom;
        public bool IsBossRoom;
        public int AliveEnemyCount;
    }

    public enum RoomState { Unexplored, Active, Cleared }
}
