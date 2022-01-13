using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletManager : MonoBehaviour
{
    private static BulletManager instance;
    public static BulletManager Instance
    {
        get
        {
            if(instance == null){
                GameObject gameObject = new GameObject("BulletManager");
                gameObject.AddComponent<BulletManager>();
            }
            return instance;
        }
    }
    
    

    [SerializeField] private int bulletsAmount;
    [SerializeField] private int playerBulletsAmount;
    [SerializeField] private int playerBulletsVelocity;
    [SerializeField] private float bulletMainSpeed;
    [SerializeField] private float minX, maxX, minY, maxY;
    [SerializeField] private float bulletSpawnDelay;
    [SerializeField] private float upgradeSpawnDelay;
    public float shootingReloadTime;
    public Slider reloadSlider;
    public Text scoreText;
    public Text bulletVelocityText;
    public Text reloatTimeText;
    public GameObject shootPointPrefab;
    public GameObject bulletPrefab;
    public GameObject playerBulletPrefab;
    public GameObject bulletsParent;
    public GameObject playerBulletsParent;
    public Transform playerShootPoint;
    public GameObject[] shieldsArray = new GameObject[6];
    public GameObject[] upgradeArray = new GameObject[4];
    public List<GameObject> houseList = new List<GameObject>();
    public List<GameObject> bulletList = new List<GameObject>();
    public List<GameObject> playerBulletList = new List<GameObject>();
    public bool tripleShotUpgradeActive{private get; set;}

    private List<Upgrades> activeUpgradesList = new List<Upgrades>();
    private PlayerBullet playerBullet;
    private MainMenu mainMenu;
    private Transform newBulletPos;
    private Vector3 mousePosition;
    private Vector2 upgradeRandomPosition;
    private int shieldActiveCount;
    private int scoreAmount;
    private int randomHouse;
    private int randomUpgrade;
    private float randomXpos;
    private float randomYpos;
    private float timeElapsed;
    private float timeToUpgrade;
    private bool canShootAgain = true;


    void Awake()
    {
        instance = this;
        mainMenu = FindObjectOfType<MainMenu>();
        SetScore(0);
    }

    void Start()
    {
        playerBullet = playerBulletPrefab.GetComponent<PlayerBullet>();
        playerBullet.bulletVelocity = playerBulletsVelocity;
        bulletVelocityText.text = playerBullet.bulletVelocity.ToString();
        reloatTimeText.text = shootingReloadTime.ToString();
        timeToUpgrade = Time.time + upgradeSpawnDelay;
        reloadSlider.maxValue = shootingReloadTime;

        for(int i = 0; i < bulletsAmount; i++){

            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.transform.parent = bulletsParent.transform;
            bulletList.Add(bullet);
            bullet.SetActive(false);
        }

        for(int i = 0; i < playerBulletsAmount; i++){

            GameObject playerBullet = Instantiate(playerBulletPrefab, transform.position, Quaternion.identity);
            playerBullet.transform.parent = playerBulletsParent.transform;
            playerBulletList.Add(playerBullet);
            playerBullet.SetActive(false);
        }
    }

    void Update()
    {
        if(Time.time >= timeElapsed){

            ShootBullet();
            timeElapsed = Time.time + bulletSpawnDelay;
        }

        if(Time.time >= timeToUpgrade){

            SpawnUpgrade();
            timeToUpgrade = Time.time + upgradeSpawnDelay;
        }

        if(Input.GetMouseButtonDown(0) && canShootAgain && !mainMenu.isMenuPaused) StartCoroutine(PlayerShoot());

        if(reloadSlider.value < reloadSlider.maxValue) reloadSlider.value += Time.deltaTime;

    }

    IEnumerator PlayerShoot()
    {   
        AudioManager.Instance.Play("LaserShoot");
        canShootAgain = false;
        reloadSlider.value = 0f;    
        mousePosition = Input.mousePosition;
        mousePosition.z = 2.0f;
        Vector3 bulletTarget = Camera.main.ScreenToWorldPoint(mousePosition);
        ShootPlayerBullet(bulletTarget);
        GameObject shootPoint = Instantiate(shootPointPrefab, bulletTarget, Quaternion.identity);
        Destroy(shootPoint, 1f);

        if(tripleShotUpgradeActive) TripleShotUpgrade();

        yield return new WaitForSeconds(shootingReloadTime);
        canShootAgain = true;
    }

    void TripleShotUpgrade()
    {
        int randomXrangeMax = Random.Range(80, 120);
        int randomXrangeMin = Random.Range(-120, -80);
        int randomYrange = Random.Range(-20, 20);
        Vector3 shootPointPosition2 = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x+randomXrangeMax, mousePosition.y-randomYrange, Camera.main.nearClipPlane));
        Vector3 shootPointPosition3 = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x-randomXrangeMin, mousePosition.y+randomYrange, Camera.main.nearClipPlane));
        ShootPlayerBullet(shootPointPosition2);
        ShootPlayerBullet(shootPointPosition3);
    }

    public void ShieldUpgrade(bool shieldActive)
    {
        for(int i = 0; i < shieldsArray.Length; i++){

            shieldsArray[i].SetActive(shieldActive);
        }
    }

    void SpawnUpgrade()
    {
        randomXpos = Random.Range(minX, maxX);
        randomYpos = Random.Range(minY, maxY);
        upgradeRandomPosition = new Vector2(randomXpos, randomYpos);


        randomUpgrade = Random.Range(0, upgradeArray.Length);

        if(upgradeArray[randomUpgrade].GetComponent<Upgrades>().GetUpgradeType() == "Shield"){

            Upgrades[] upgrades = FindObjectsOfType(typeof(Upgrades)) as Upgrades[];
            if(upgrades.Length > 0)
            {
                foreach(Upgrades upgrade in upgrades){

                    activeUpgradesList.Add(upgrade);
                }

                for(int i = 0; i < activeUpgradesList.Count; i++){

                    if(activeUpgradesList[i].isShieldActive){

                        SpawnUpgrade();
                        shieldActiveCount = 0;
                        break;
                    }
                    else
                    {
                        shieldActiveCount++;
                    }
                }

                if(shieldActiveCount == activeUpgradesList.Count){

                    InstantiateUpgrade();
                    shieldActiveCount = 0;
                }
            }
            else InstantiateUpgrade();

        }
        else InstantiateUpgrade();

    }
    void InstantiateUpgrade()
    {
        GameObject upgrade = Instantiate(upgradeArray[randomUpgrade], upgradeRandomPosition, Quaternion.identity);
        Destroy(upgrade, 15f);
    }

    public void SetBulletVelocityUpgrade(float velocity)
    {
        playerBullet.bulletVelocity += velocity;
        bulletVelocityText.text = playerBullet.bulletVelocity.ToString();
    }
    public void SetReloadTimeUpgrade(float time)
    {
        if(shootingReloadTime > 0.2){

            shootingReloadTime -= time;
            reloadSlider.maxValue = shootingReloadTime;
            reloatTimeText.text = shootingReloadTime.ToString();
        }
    }

    void ShootBullet()
    {
        for(int i = 0; i < bulletList.Count; i++)
        {
            if(!bulletList[i].activeInHierarchy){

                bulletList[i].SetActive(true);
                
                randomXpos = Random.Range(minX, maxX);
                bulletList[i].transform.localPosition = new Vector2(randomXpos, 0f);

                if(houseList.Count > 0){

                    randomHouse = Random.Range(0, houseList.Count);
                    bulletList[i].GetComponent<Bullet>().SetTarget(houseList[randomHouse]);
                }
                
                break;
            }
        }
    }

    void ShootPlayerBullet(Vector3 bulletTarget)
    {
        for(int i = 0; i < playerBulletList.Count; i++)
        {
            if(!playerBulletList[i].activeInHierarchy){

                playerBulletList[i].SetActive(true);
                playerBulletList[i].transform.position = playerShootPoint.position;
                playerBulletList[i].GetComponent<PlayerBullet>().SetTarget(bulletTarget);
                break;
            }
        }
    }

    public void HouseDestroy(GameObject houseObject)
    {
        StartCoroutine(HouseDestroyed(0.5f, houseObject));
    }

    IEnumerator HouseDestroyed(float timeToDestroy, GameObject houseHitObj)
    {
        SetScore(-100);
        houseHitObj.GetComponent<Animator>().SetTrigger("Destroy");
        houseHitObj.GetComponent<BoxCollider2D>().enabled = false;
        AudioManager.Instance.Play("HouseExplosion");
        yield return new WaitForSeconds(timeToDestroy);
        houseHitObj.SetActive(false);
        houseList.Remove(houseHitObj);
    }

    public void SetScore(int score)
    {
        scoreAmount += score;
        scoreText.text = scoreAmount.ToString();
        if(score > 0 && scoreAmount > 0 && bulletSpawnDelay > 0.75f){

            var scoreTemp = scoreAmount / 100000f;
            bulletSpawnDelay -= scoreTemp;

            for(int i = 0; i < bulletList.Count; i++){

                Bullet bullet = bulletList[i].GetComponent<Bullet>();
                if(bullet.speed < 4) bullet.speed += scoreTemp;
            }
        }

    }
}
