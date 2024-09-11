using UnityEngine;
using TMPro;

public class INodeView : MonoBehaviour
{
   [SerializeField] private TMP_Text _nameField;
   [SerializeField] private LineRendererUI _lrFab;

   public void Init(string name) {
        _nameField.text = name;
   } 

   public void DrawLine(Vector2 toPos) {
        LineRendererUI lr = Instantiate(_lrFab.gameObject, transform).GetComponent<LineRendererUI>();
        lr.CreateLine(transform.position, toPos);
   }
}
