using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EmerginPointsPool : MonoBehaviour
{
    private Queue<EmergingPoints> usedPoints;
    private Transform container;
    [SerializeField] private GameObject emergingPointTemplate;

    private void Awake()
    {
        usedPoints = new Queue<EmergingPoints>();
    }


    public EmergingPoints GetText()
    {
        EmergingPoints point = FindPoints();
        if (point != null)
            return point;
        GameObject newPoint = Instantiate(emergingPointTemplate, transform);
        EmergingPoints textComp = newPoint.GetComponent<EmergingPoints>();
        InsertNewPoint(textComp);
        return textComp;
    }

    private EmergingPoints FindPoints()
    {
        int pointsCount = usedPoints.Count;
        for (int i = 0; i < pointsCount; i++)
        {
            EmergingPoints point = usedPoints.Dequeue();
            usedPoints.Enqueue(point);
            if (point.GetIsReadyToUse())
            {
                point.SetIsReadyToUse(false);
                return point;
            }
        }

        return null;
    }

    private void InsertNewPoint(EmergingPoints newPoint) =>
        usedPoints.Enqueue(newPoint);
}