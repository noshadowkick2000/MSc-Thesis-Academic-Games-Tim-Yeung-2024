using UnityEngine;
using UnityEngine.Serialization;

public class TrialObject : MonoBehaviour
{
    [FormerlySerializedAs("mainSprite")] [SerializeField] private SpriteRenderer mainSpriteRenderer;
    private bool animated = false;
    private Sprite[] sprites;
    public SpriteRenderer MainSpriteRenderer => mainSpriteRenderer;

    public void SetSprite(Sprite sprite)
    {
        mainSpriteRenderer.sprite = sprite;
    }

    public void SetSprites(Sprite[] animationSprites)
    {
        animated = true;
        sprites = animationSprites;
        mainSpriteRenderer.sprite = animationSprites[0];
    }

    private int counter;
    private void FixedUpdate()
    {
        if (!animated) return;

        mainSpriteRenderer.sprite = sprites[counter];
        
        if (counter < sprites.Length - 1)
            counter++;
        else
            counter = 0;
    }
}
