using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;

public class Circle
{
    public Vector2 mCenter;
    public float mRadius;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="iCenter"></param>
    /// <param name="Radius"></param>
    public Circle(Vector2 iCenter, float Radius)
    {
        mCenter = iCenter;
        mRadius = Radius;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return "Rad: " + mRadius + " _ Center: " + mCenter.ToString();
    }
}

public class CirclePacker
{
    public List<Circle> mCircles = new List<Circle>();
    public Circle mDraggingCircle = null;
    protected Vector2 mPackingCenter;
    public float mMinSeparation = 1f;

    /// <summary>
    /// Generates a number of Packing circles in the constructor.
    /// Random distribution is linear
    /// </summary>
    public CirclePacker(Vector3 pPackingCenter, int pNumCircles,
                        double pMinRadius, double pMaxRadius)
    {
        this.mPackingCenter = new Vector2(pPackingCenter.x, pPackingCenter.z);

        // Create random circles
        this.mCircles.Clear();
        Random Rnd = new Random(System.DateTime.Now.Millisecond);
        for (int i = 0; i < pNumCircles; i++)
        {
            Vector2 nCenter = new Vector2((float)(this.mPackingCenter.x +
                                                  Rnd.NextDouble() * pMinRadius),
                                          (float)(this.mPackingCenter.y +
                                                  Rnd.NextDouble() * pMinRadius));
            float nRadius = (float)pMinRadius;// (pMinRadius + Rnd.NextDouble() *
                                              //(pMaxRadius - pMinRadius));
            this.mCircles.Add(new Circle(nCenter, nRadius));
        }

        //Vector2[] vecs = new Vector2[] { new Vector2(0f, 0f), new Vector2(.5f, .2f), new Vector2(.35f, -0.5f) };


        //for (int i = 0; i < 3; i++)
        //{
        //    Vector2 nCenter = vecs[i];
        //    float nRadius = (float)pMinRadius;// (pMinRadius + Rnd.NextDouble() *
        //                                      //(pMaxRadius - pMinRadius));
        //    this.mCircles.Add(new Circle(nCenter, nRadius));
        //}
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="?"></param>
    /// <returns></returns>
    private float DistanceToCenterSq(Circle pCircle)
    {
        return (pCircle.mCenter - mPackingCenter).sqrMagnitude;
    }
    /// <summary>
    ///
    /// </summary>
    private int Comparer(Circle p1, Circle P2)
    {
        float d1 = DistanceToCenterSq(p1);
        float d2 = DistanceToCenterSq(P2);
        if (d1 < d2)
            return -1;
        else if (d1 > d2)
            return 1;
        else return 0;
    }
    /// <summary>
    ///
    /// </summary>
    public bool Update()
    {
        // Sort circles based on the distance to center
        mCircles.Sort(Comparer);
        bool updated = false;

        float minSeparationSq = mMinSeparation * mMinSeparation;
        for (int i = 0; i < mCircles.Count - 1; i++)
        {
            for (int j = i + 1; j < mCircles.Count; j++)
            {
                if (i == j)
                    continue;

                Vector2 AB = mCircles[j].mCenter - mCircles[i].mCenter;
                float r = mCircles[i].mRadius + mCircles[j].mRadius;

                // Length squared = (dx * dx) + (dy * dy);
                float d = AB.sqrMagnitude - minSeparationSq;
                float minSepSq = Math.Min(d, minSeparationSq);
                d -= minSepSq;

                if (d < (r * r) - 0.01)
                {
                    updated = true;
                    AB.Normalize();
                    AB *= (float)((r - Math.Sqrt(d)) * 0.5f);
                    //if (mCircles[j] != mDraggingCircle)
                        mCircles[j].mCenter += AB;
                    //if (mCircles[i] != mDraggingCircle)
                        mCircles[i].mCenter -= AB;
                }
            }
        }

        return updated;
    }
}
