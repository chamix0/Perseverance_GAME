using System.Collections.Generic;
using UnityEngine;

public class LaberinthWalls : MonoBehaviour
{
    private DontTouchWallsManager dontTouchWallsManager;
    private List<Wall> walls;
    [SerializeField] private GameObject wallContainer;
    [SerializeField] private Wall spawnPos;
    [SerializeField] private Wall endWall;
    private bool active;

    private void Awake()
    {
        walls = new List<Wall>();
    }

    private void Start()
    {
        dontTouchWallsManager = FindObjectOfType<DontTouchWallsManager>();
        walls.AddRange(wallContainer.GetComponentsInChildren<Wall>());
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (CheckColision())
                dontTouchWallsManager.KillPlayer();
            if (CheckIfEnded())
                dontTouchWallsManager.EndRound();
        }
    }

    public void DisableLaberinth()
    {
        active = false;
        if (walls.Count==0)
            walls.AddRange(wallContainer.GetComponentsInChildren<Wall>());
        
        foreach (var wall in walls)
            wall.Hide();
        spawnPos.Hide();
        endWall.Hide();
    }

    public void ShowLaberinth()
    {
        active = true;
        if (walls.Count==0)
            walls.AddRange(wallContainer.GetComponentsInChildren<Wall>());
        foreach (var wall in walls)
            wall.Show();
        spawnPos.Show();
        endWall.Show();
    }

    private bool CheckIfEnded()
    {
        Vector2 playerPos = dontTouchWallsManager.PlayerPos();
        if (endWall.IsPlayerInWall(playerPos))
            return true;
        return false;
    }


    public Vector2 GetSpawnPoint()
    {
        return spawnPos.GetPos();
    }

    private bool CheckColision()
    {
        Vector2 playerPos = dontTouchWallsManager.PlayerPos();

        foreach (var wall in walls)
        {
            if (wall.IsPlayerInWall(playerPos))
                return true;
        }

        return false;
    }
}