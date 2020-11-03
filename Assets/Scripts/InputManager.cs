using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private bool InputOpen = true;

    void Update()
    {
        if (InputOpen)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)) as PolygonCollider2D;
                if (hit != null && hit.CompareTag("Cell"))
                {
                    Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) -
                                                    hit.gameObject.transform.position;
                    GameManager.instance.Board.CellPres(hit.gameObject.GetComponent<Cell>() , Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    //GameManager.instance.Board.CellPressed(hit.gameObject.GetComponent<Cell>() , direction);
                }
            }
            
        }
    }
}
