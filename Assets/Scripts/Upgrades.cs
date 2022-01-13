using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    Shield,
    Reload,
    BulletVelocity,
    Triple,
}

public class Upgrades : MonoBehaviour, IUpgrade
{
    public float upgradeTime;
    public bool isShieldActive;
    public UpgradeType upgradeType;
    private GameObject shieldsObject;
    private BoxCollider2D boxCollider2D;
    private SpriteRenderer spriteRenderer;
    private List<GameObject> shieldList = new List<GameObject>();

    private float reloadTimeUpgrade = 0.15f;
    private float bulletVelocityUpgrade = 2f;

    void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        shieldsObject = GameObject.FindGameObjectWithTag("Shields");
        foreach(Transform child in shieldsObject.transform){

            shieldList.Add(child.gameObject);
        }
    }

    public void SetUpgrade()
    {
        switch(upgradeType.ToString())
        {
            case "Shield":
                StartCoroutine(ShieldUpgrade());
                break;
            case "Triple":
                StartCoroutine(TripleUpgrade());
                break;
            case "Reload":
                ReloadUpgrade();
                break;
            case "BulletVelocity":
                BulletUpgrade();
                break;
        }
        spriteRenderer.color = new Color32(255,255,255,0);
    }

    IEnumerator ShieldUpgrade()
    {
        boxCollider2D.enabled = false;
        BulletManager.Instance.ShieldUpgrade(true);
        yield return new WaitForSeconds(upgradeTime);
        BulletManager.Instance.ShieldUpgrade(false);
        Destroy(gameObject,0.2f);
    }
    IEnumerator TripleUpgrade()
    {
        boxCollider2D.enabled = false;
        BulletManager.Instance.tripleShotUpgradeActive = true;
        yield return new WaitForSeconds(upgradeTime);
        BulletManager.Instance.tripleShotUpgradeActive = false;
        Destroy(gameObject,0.2f);
    }
    void ReloadUpgrade()
    {
        BulletManager.Instance.SetReloadTimeUpgrade(reloadTimeUpgrade);
        Destroy(gameObject);
    }
    void BulletUpgrade()
    {
        BulletManager.Instance.SetBulletVelocityUpgrade(bulletVelocityUpgrade);
        Destroy(gameObject);
    }

    public string GetUpgradeType()
    {
        return upgradeType.ToString();
    }
}
