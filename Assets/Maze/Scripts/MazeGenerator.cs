﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;
    public GameObject finish;
    public GameObject nodePrefab;
    public float nodeWidth;
    public int mazeSize;
    public float playerSpawnHeight;
    public Material portalMaterial;

    private Node[,] nodes;

    void Awake ()
    {
        // create a floor to fit the maze
        GameObject floor = transform.Find("Floor").gameObject;
        floor.transform.parent = transform;
        floor.transform.position = transform.position;
        floor.transform.localScale = new Vector3(mazeSize, 1, mazeSize);
        floor.transform.rotation = transform.rotation;

        // change the tiling of the floor texture
        floor.GetComponent<Renderer>().material.mainTextureScale = new Vector2(mazeSize, mazeSize);

        // this is to center the maze on the parent's position
        Vector3 offset = new Vector3((float)(mazeSize - 1) / 2 * nodeWidth, 0, (float)(mazeSize - 1) / 2 * nodeWidth);

        // instantiate nodes
        nodes = new Node[mazeSize, mazeSize];
        for (int x = 0; x < mazeSize; x++)
        {
            for (int y = 0; y < mazeSize; y++)
            {
                nodes[x, y] = Instantiate(nodePrefab).GetComponent<Node>();
                nodes[x, y].transform.parent = transform;
                nodes[x, y].transform.localPosition = new Vector3(transform.position.x - offset.x + x * nodeWidth,
                    transform.position.y + 0, transform.position.z - offset.z + y * nodeWidth);
                nodes[x, y].transform.rotation = transform.rotation;
            }
        }

        // link nodes
        for (int x = 0; x < mazeSize; x++)
        {
            for (int y = 0; y < mazeSize; y++)
            {
                // link left node
                if (x > 0)
                {
                    nodes[x, y].Left = nodes[x - 1, y];
                }

                // link right node
                if (x < mazeSize - 1)
                {
                    nodes[x, y].Right = nodes[x + 1, y];
                }

                // link up node
                if (y < mazeSize - 1)
                {
                    nodes[x, y].Up = nodes[x, y + 1];
                }

                // link down node
                if (y > 0)
                {
                    nodes[x, y].Down = nodes[x, y - 1];
                }
            }
        }

        // make a maze!
        DFSMaze();

        // place the player in the maze
        player.transform.position = nodes[0, 0].transform.position + Vector3.up * playerSpawnHeight;

        // place a portal in the maze
        GameObject portal = null;
        while (portal == null)
        {
            int x = Random.Range(0, mazeSize);
            int y = Random.Range(0, mazeSize);
            int dir = Random.Range(0, 4);

            switch (dir)
            {
                case 0:
                    portal = nodes[x, y].leftWall;
                    break;
                case 1:
                    portal = nodes[x, y].rightWall;
                    break;
                case 2:
                    portal = nodes[x, y].topWall;
                    break;
                case 3:
                    portal = nodes[x, y].bottomWall;
                    break;
                default:
                    portal = null;
                    break;
            }
        }
        portal.GetComponent<Renderer>().material = portalMaterial;
        portal.tag = "Portal";

        // also place the finish and bad guy in the maze
        finish.transform.position = enemy.transform.position = nodes[mazeSize - 1, mazeSize - 1].transform.position;
    }

    // http://www.algosome.com/articles/maze-generation-depth-first.html
    private void DFSMaze ()
    {
        Queue<Node> Q = new Queue<Node>();

        // 1. Randomly select a node (or cell) N. 
        Node N = nodes[Random.Range(0, mazeSize), Random.Range(0, mazeSize)];

        while (true)
        {
            // 2. Push the node N onto a queue Q.
            Q.Enqueue(N);

            // 3. Mark the cell N as visited. 
            N.Visited = true;

            Direction neighborDirection;
            do
            {
                // 4. Randomly select an adjacent cell A of node N that has not been visited.
                neighborDirection = FindNeighborDirection(N);

                // If all the neighbors of N have been visited: 
                if (neighborDirection == Direction.None)
                {
                    // Continue to pop items off the queue Q until a node is encountered with at least one non-visited neighbor.
                    // Assign this node to N and go to step 4.
                    if (Q.Count > 0)
                    {
                        N = Q.Dequeue();
                    }
                    else
                    {
                        // If no nodes exist: stop.
                        return;
                    }
                }
            }
            while (neighborDirection == Direction.None);

            // 5. Break the wall between N and A.
            Node A = null;
            switch (neighborDirection)
            {
                case Direction.Left:
                    A = N.Left;
                    N.DestroyLeftWall();
                    A.DestroyRightWall();
                    break;
                case Direction.Right:
                    A = N.Right;
                    N.DestroyRightWall();
                    A.DestroyLeftWall();
                    break;
                case Direction.Up:
                    A = N.Up;
                    N.DestroyTopWall();
                    A.DestroyBottomWall();
                    break;
                case Direction.Down:
                    A = N.Down;
                    N.DestroyBottomWall();
                    A.DestroyTopWall();
                    break;
            }

            // 6. Assign the value A to N.
            N = A;

            // 7. Go to step 2.
        }
    }

    private static Direction FindNeighborDirection (Node N)
    {
        Direction neighborDirection;
        List<Direction> directions = new List<Direction> { Direction.Left, Direction.Right, Direction.Up, Direction.Down };
        while (directions.Count > 0)
        {
            Node A = null;
            int randomIndex = Random.Range(0, directions.Count);
            neighborDirection = directions[randomIndex];
            switch (neighborDirection)
            {
                case Direction.Left:
                    A = N.Left;
                    break;
                case Direction.Right:
                    A = N.Right;
                    break;
                case Direction.Up:
                    A = N.Up;
                    break;
                case Direction.Down:
                    A = N.Down;
                    break;
            }

            if (A != null && A.Visited == false)
            {
                // found an unvisited node!
                return neighborDirection;
            }

            // this node is visited, or null!
            // remove from possible directions
            directions.RemoveAt(randomIndex);
        }

        // checked all directions, no neighbors
        return Direction.None;
    }

    private enum Direction
    {
        None, Left, Right, Up, Down
    }
}
