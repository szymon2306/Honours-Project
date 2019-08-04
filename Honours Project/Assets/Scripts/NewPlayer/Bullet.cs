using UnityEngine;
public class Bullet : MonoBehaviour
{
    public FPSController owner;
    public float damage;
    [SerializeField]
    private GameObject bulletHolePrefab;
    public void ShootBullet(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity))
        {

            //Debug.Log(hit.transform.name);
            ClassicEnemy enemyHit = hit.transform.GetComponent<ClassicEnemy>();
            if (enemyHit != null)
            {

                enemyHit.TakeDamage(damage);
            }
            GameObject tempHole = Instantiate(bulletHolePrefab, hit.point, Quaternion.FromToRotation(Vector3.back, hit.normal));
            tempHole.transform.localPosition -= tempHole.transform.forward * .01f;
            tempHole.transform.SetParent(hit.transform);

        }
        Debug.DrawLine(transform.position, transform.position + direction * 100, Color.green, 3);
        Destroy(this.gameObject);
    }
}