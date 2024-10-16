using UnityEngine;

public class PlanetAction : MonoBehaviour
{

    private Transform selection;
    private RaycastHit raycastHit;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Ray ray  = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool c = Physics.Raycast(ray, out raycastHit);
        if(Input.GetKey(KeyCode.Mouse0) /* && !EventSystem.current.IsPointerOverGameObject() */) {
            selection = raycastHit.transform;
            // Debug.Log(typeof selection.gameObject).SendMessage("");
            GameObject p = selection.GetComponent<GameObject>();
            Debug.Log(selection, p);
            if(p != null) {
                p.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
        }
    }
}
