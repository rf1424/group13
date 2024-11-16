using System.Collections.Generic;
using UnityEngine;
// [RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour
{
    [Header("Tile Materials")]
    //[SerializeField] private Sprite unclickedTile;
    //[SerializeField] private Sprite flaggedTile;
    [SerializeField] private List<Material> clickedTiles;
    //[SerializeField] private Sprite mineTile;
    //[SerializeField] private Sprite mineWrongTile;
    //[SerializeField] private Sprite mineHitTile;
    
    // private SpriteRenderer spriteRenderer;
    public bool flagged = false;
    public bool active = true;
    public bool isMine = false;
    public int mineCount = 0;


    [SerializeField] private GameObject tile;
    [SerializeField] private Material unclickedMaterial;
    [SerializeField] private Material flagMaterial;
    [SerializeField] private Material mineHitTileMaterial;
    [SerializeField] private Material emptyMaterial;

    [SerializeField] private Texture flagTexture;

    [SerializeField] private Material oneMaterial;
    [SerializeField] private Material twoMaterial;
    [SerializeField] private Material threeMaterial;
    [SerializeField] private Material fourMaterial;
    [SerializeField] private Material fiveMaterial;
    [SerializeField] private Material sixMaterial;
    [SerializeField] private Material sevenMaterial;
    [SerializeField] private Material eightMaterial;

    [SerializeField] private Material mineMaterial;
    [SerializeField] private Material mineWrongMaterial;

    private Renderer tileRenderer;
    private MaterialPropertyBlock propertyBlock;

    private Material[] numbers;

    public GameManagerMS gameManagerMS;


    void Awake()
    {
        // This should always exist due to the RequireComponent helper.
        // spriteRenderer = GetComponent<SpriteRenderer>();
        tileRenderer = GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();
        numbers = new Material[] {null, oneMaterial, twoMaterial, threeMaterial, fourMaterial, fiveMaterial, sixMaterial, sevenMaterial, eightMaterial};
    }

    private void Update()
    {
    }

    

    public void ClickTile()
    {
        // Don't allow left clicks on flags.
        if (active & !flagged)
        {
            // Ensure it can no longer be pressed again.
            active = false;
            if (isMine)
            {
                // Game over :(
                tileRenderer.material = new Material(mineHitTileMaterial);

                //gameManager.GameOver();
            }
            else
            {

                // it was a safe click, set the correct sprite.
                if (mineCount == 0)
                {
                    tileRenderer.material = new Material(emptyMaterial);
                    // Register that the click should expand out to the neighbours.
                    gameManagerMS.ClickNeighbours(this);
                }
                else
                {
                    // Debug.Log("num mines: " + mineCount);
                    Material num = numbers[mineCount];
                    // Debug.Log(num == null);
                    tileRenderer.material = new Material(numbers[mineCount]);
                }

                // Whenever we successfully make a change check for game over.
                gameManagerMS.CheckGameOver();
            }
        }
    }

    public void FlagTile()
    {
        Debug.Log("right clicked");

        // If right click toggle flag on/off.
        flagged = !flagged;
        if (flagged)
        {
            Debug.Log("flag");
            //SetMaterial(flagMaterial);
            tileRenderer.material = new Material(flagMaterial);
            // spriteRenderer.sprite = flaggedTile;
        }
        else
        {
            // spriteRenderer.sprite = unclickedTile;
            Debug.Log("unflag");
            //SetMaterial(unclickedMaterial);
            tileRenderer.material = new Material(unclickedMaterial);
        }

    }

    // If this tile should be shown at game over, do so.
    public void ShowGameOverState()
    {
        if (active)
        {
            active = false;
            if (isMine & !flagged)
            {
                // If mine and not flagged show mine.
                tileRenderer.material = new Material(mineMaterial);
            }
            else if (flagged & !isMine)
            {
                // If flagged incorrectly show crossthrough mine
                tileRenderer.material = new Material(mineWrongMaterial);
            }
        }
    }

    // Helper function to flag remaning mines on game completion.
    public void SetFlaggedIfMine()
    {
        if (isMine)
        {
            flagged = true;
            tileRenderer.material = new Material(flagMaterial);
        }
    }

    private void SetMaterial(Material material)
    {
        propertyBlock.SetTexture("_MainTex", material.mainTexture);
        tileRenderer.SetPropertyBlock(propertyBlock);
    }

}