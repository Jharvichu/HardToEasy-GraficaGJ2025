using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour 
{
    [SerializeField] private List<Waypoint> _neighbors; 

    public Waypoint GetRandomNeighbor(Waypoint previousNode = null)
    {
        if (_neighbors.Count == 0) return null;

        if (_neighbors.Count > 1 && previousNode != null && _neighbors.Contains(previousNode))
        {
            List<Waypoint> options = new List<Waypoint>(_neighbors);
            options.Remove(previousNode);
            return options[Random.Range(0, options.Count)];
        }

        return _neighbors[Random.Range(0, _neighbors.Count)];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach (var neighbor in _neighbors)
        {
            if (neighbor != null) Gizmos.DrawLine(transform.position, neighbor.transform.position);
        }
    }
}