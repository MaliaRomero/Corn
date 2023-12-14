using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviourPun
{
    // Start is called before the first frame update
    public Sprite ammoFullSprite;   // Sprite representing full ammo
    public Sprite ammoEmptySprite;  // Sprite representing empty ammo
    public int maxAmmo = 10;        // Maximum ammo count
    public int gridColumns = 5;     // Number of columns in the grid
    public int gridRows = 2;        // Number of rows in the grid

    private Image[] ammoImages;     // Array to hold ammo sprites
    private int currentAmmo;        // Current ammo count

    void Start()
    {
        if (!photonView.IsMine)
            return;

        currentAmmo = maxAmmo;
        InitializeAmmoUI();
    }

    void InitializeAmmoUI()
    {
        int ammoCount = gridColumns * gridRows;
        ammoImages = new Image[ammoCount];

        GridLayoutGroup gridLayout = GetComponent<GridLayoutGroup>();
        float cellWidth = gridLayout.cellSize.x;
        float cellHeight = gridLayout.cellSize.y;

        for (int i = 0; i < ammoCount; i++)
        {
            GameObject ammoObj = new GameObject("Ammo" + i);
            ammoObj.transform.SetParent(transform);
            ammoObj.AddComponent<RectTransform>();

            Image image = ammoObj.AddComponent<Image>();
            image.sprite = ammoFullSprite;  // Set initial sprite to represent full ammo
            ammoImages[i] = image;

            // Calculate grid layout positions
            int row = i / gridColumns;
            int col = i % gridColumns;
            ammoObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(col * cellWidth, -row * cellHeight);
        }

        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        if (ammoImages == null)
            return;

        for (int i = 0; i < ammoImages.Length; i++)
        {
            if (i < currentAmmo)
            {
                ammoImages[i].sprite = ammoFullSprite;
            }
            else
            {
                ammoImages[i].sprite = ammoEmptySprite;
            }
        }
    }
}
