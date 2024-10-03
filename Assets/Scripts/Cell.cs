using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
		 public int width = 256;    // Width of the grid
    public int height = 256;   // Height of the grid
    public float obstacleRadius = 30.0f; // Radius of the circular obstacle
    public Vector2 obstaclePosition; // Position of the circular obstacle
    
    private float[,] density;  // Fluid density
    private Vector2[,] velocity; // Fluid velocity
    private Texture2D texture; // Texture to visualize the simulation
    private Renderer rend;

    private void Start()
    {
        obstaclePosition = new Vector2(width / 2, height / 2); // Set obstacle in the center

        // Initialize the grid
        density = new float[width, height];
        velocity = new Vector2[width, height];

        texture = new Texture2D(width, height);
        rend = GetComponent<Renderer>();
        rend.material.mainTexture = texture;

        InitializeGrid();
    }

    private void InitializeGrid()
    {
        // Initialize density and velocity
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                density[x, y] = Random.Range(0f, 1f); // Random initial density
                velocity[x, y] = new Vector2(0.1f, 0.1f); // Small initial velocity
                
               
            }
        }
    }

    private void Update()
    {
        // Update the Lattice Boltzmann grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Skip the obstacle cells
                if (IsInsideObstacle(x, y)) continue;

                // Collision model (very basic)
                Vector2 vel = velocity[x, y];
                vel *= 0.99f; // Simple damping for velocity
                velocity[x, y] = vel;

                // Streaming step (advection)
                int newX = (int)(x + vel.x) % width;
                int newY = (int)(y + vel.y) % height;

                // Wrap around grid boundaries
                if (newX < 0) newX += width;
                if (newY < 0) newY += height;

                density[newX, newY] = density[x, y];
            }
        }

        // Visualize the grid
        UpdateTexture();
    }

    private bool IsInsideObstacle(int x, int y)
    {
        float dx = x - obstaclePosition.x;
        float dy = y - obstaclePosition.y;
        return dx * dx + dy * dy < obstacleRadius * obstacleRadius;
    }

    private void UpdateTexture()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float d = density[x, y];
                Color color = new Color(d, d, d); // Color based on density
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
    }

    private void OnDrawGizmos()
    {
        // Draw circular obstacle
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(obstaclePosition.x, obstaclePosition.y, 0), obstacleRadius);
    }
}
