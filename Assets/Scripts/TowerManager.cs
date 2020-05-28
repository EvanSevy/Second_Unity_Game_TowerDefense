using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TowerManager : Singleton<TowerManager> {

    public TowerBtn towerBtnPressed { get; set; }
    private SpriteRenderer spriteRenderer;

    private List<Tower> TowerList = new List<Tower>();
    private List<Collider2D> BuildList = new List<Collider2D>();
    private Collider2D buildTile;

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        buildTile = GetComponent<Collider2D>();
        spriteRenderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            if(hit.collider.tag == "BuildSite")
            {
                buildTile = hit.collider;
                buildTile.tag = "buildSiteFull";
                //hit.collider.tag = "buildSiteFull";
                RegisterBuildSite(buildTile);
                placeTower(hit);
            }
        }
        if (spriteRenderer.enabled)
        {
            followMouse();
        }
    }
    public void RegisterBuildSite(Collider2D buildTag)
    {
        BuildList.Add(buildTag);
    }
    public void RegisterTower(Tower item)
    {
        TowerList.Add(item);
    }
    public void RenameTagsBuildSites()
    {
        foreach(Collider2D buildTag in BuildList)
        {
            buildTag.tag = "BuildSite";
        }
        BuildList.Clear();
    }
    public void DestroyAllTowers()
    {
        foreach(Tower tower in TowerList)
        {
            Destroy(tower.gameObject);
        }
        TowerList.Clear();
    }
    public void placeTower(RaycastHit2D hit)
    {
        if(!EventSystem.current.IsPointerOverGameObject() && towerBtnPressed != null) {
            Tower newTower = Instantiate(towerBtnPressed.TowerObject);
            BuyTower(towerBtnPressed.TowerPrice);
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.TowerBuilt);
            RegisterTower(newTower);
            newTower.transform.position = hit.transform.position;
            disableDragSprite();
        }
    }
    public void BuyTower(int price)
    {
        GameManager.Instance.SubtractMoney(price);
    }
    public void selectedTower(TowerBtn towerSelected)
    {
        if (towerSelected.TowerPrice <= GameManager.Instance.TotalMoney)
        {
            towerBtnPressed = towerSelected;
            enableDragSprite(towerBtnPressed.DragSprite);
        }
        
    }

    public void followMouse()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(transform.position.x, transform.position.y);
    }

    public void enableDragSprite(Sprite sprite)
    {
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = sprite;
    }
    public void disableDragSprite()
    {
        spriteRenderer.enabled = false;
    }
}
