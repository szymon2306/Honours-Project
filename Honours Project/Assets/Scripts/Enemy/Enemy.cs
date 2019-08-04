using UnityEngine;
public class ClassicEnemy : MonoBehaviour
{
    [SerializeField]
    private float health;
    public void TakeDamage(float damageIn)
    {
        health -= damageIn;
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}