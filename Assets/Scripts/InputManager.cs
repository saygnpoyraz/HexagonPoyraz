using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    private bool input = true;
    
    private Vector2 clickPos;


    private void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (input)
        {
            if (Input.GetMouseButtonDown(0))
            {
                clickPos = Input.mousePosition;
            }

            if (Input.GetMouseButton(0))
            {
                Vector2 mousePos = Input.mousePosition;
                if (mousePos.y - clickPos.y > 10)
                {
                    GameManager.instance.board.RotateSelectedCells(false,3);
                    input = false;
                    // Rotate CounterClockWise
                }

                if (mousePos.y - clickPos.y < -10)
                {
                    GameManager.instance.board.RotateSelectedCells(true,3);
                    input = false;
                    // Rotate Clockwise
                }

            }
            if (Input.GetMouseButtonUp(0))
            {
                var hit = Physics2D.OverlapPoint(GameManager.instance.mainCamera.ScreenToWorldPoint(Input.mousePosition)) as PolygonCollider2D;
                if (hit != null && hit.CompareTag("Cell"))
                {
                    GameManager.instance.board.CellPressed(hit.gameObject.GetComponent<Cell>() , Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
            }
        }
    }

    public void OpenInput()
    {
        input = true;
    }

    public void CloseInput()
    {
        input = false;
    }
}
