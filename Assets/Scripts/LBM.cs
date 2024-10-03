using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBM : MonoBehaviour{
    private const int xDimension = 400;
    private const int yDimension = 100;
    private const float tau = .53f;
    private const int Nt = 3000;

    // lattice vel and weights
    private const int NL = 9;
    private static readonly int[] cxs = new int[] {0, 0, 1, 1, 1, 0, -1, -1, -1};
    private static readonly int[] cys = new int[] {0, 1, 1, 0, -1, -1, -1, 0, -1};
    private static readonly float[] weights = new float[] {0.444f, 0.111f, 0.027f, 0.111f, 0.027f, 0.111f, 0.027f, 0.111f, 0.027f};
    
    
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
