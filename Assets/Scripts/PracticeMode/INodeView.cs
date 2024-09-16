using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class INodeView : MonoBehaviour
{
   [SerializeField] private TMP_Text _nameField;
   [SerializeField] private LineRendererUI _lrFab;
   [SerializeField] private Image _bgImage;
   [SerializeField] private Image _fileImage;

   public void Init(string name, Sprite spr) {
        _nameField.text = name;
        _fileImage.sprite = spr;
   } 

   public void SetBackgroundColor(Color col) {
     _bgImage.color = col;
   }

   public GameObject DrawLine(Vector2 toPos) {
        LineRendererUI lr = Instantiate(_lrFab.gameObject, transform).GetComponent<LineRendererUI>();
        lr.CreateLine(transform.position, toPos);
        return lr.gameObject;
   }
}
