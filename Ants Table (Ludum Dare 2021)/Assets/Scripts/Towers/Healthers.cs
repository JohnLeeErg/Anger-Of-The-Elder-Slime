using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthers : MonoBehaviour
{
    [SerializeField] protected float maxHealth = 100;
    [SerializeField] protected float health;
    [SerializeField] protected float damage = 1;
    [SerializeField] protected float healthBarOffset = .1f;
    [SerializeField] private GameObject healthBar;
    [SerializeField] public string deathAnimation;

    private Transform redBar;

    void Awake()
    {
        GameObject bar = Instantiate(healthBar, gameObject.transform);
        float spriteHieght = GetComponent<SpriteRenderer>().sprite.rect.y/2;

        bar.transform.position = new Vector3(bar.transform.position.x, bar.transform.position.y + spriteHieght+ healthBarOffset, bar.transform.position.z);
        redBar = bar.transform.GetChild(1);

        healthBar.SetActive(false);
    }


    public void Damage(float dmg)
    {
        health -= dmg;
        UpdateHealthBar();
        if (health <= 0)
        {
            if (GetComponent<SlimePathfinding>()) {
                GetComponent<SlimePathfinding>().StopAllCoroutines();
                GetComponent<SlimePathfinding>().enabled = false;
            }
            if (GetComponent<BoxCollider2D>()) {
                GetComponent<BoxCollider2D>().enabled = false;
            }

            if (GetComponent<CapsuleCollider2D>()) {
                GetComponent<CapsuleCollider2D>().enabled = false;
            }

            if (GetComponent<GridObject>())
            {
                GetComponent<GridObject>().enabled = false;
            }

            redBar.parent.gameObject.SetActive(false);

            //removes from grid
            if (GridManagement.IsInGrid(gameObject))
            {
                GridManagement.ClearPosition(GridManagement.GetPositionOfObject(gameObject), true);
            }
            GetComponent<Animator>().Play(deathAnimation);
            float length = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
            DeathHelper();
            StartCoroutine(DelayedDeath(length));
        }
    }

    public virtual void Death()
    {
        Destroy(gameObject);
    }

    public float GetDamage() {
        return damage;
    }

    public virtual void DeathHelper()
    {
        //do nothing
    }

    private void UpdateHealthBar() {
        if (health == maxHealth)
            redBar.parent.gameObject.SetActive(false);
        else
            redBar.parent.gameObject.SetActive(true);

        float percent = Mathf.Clamp((health / maxHealth), 0, 100);
        redBar.localScale = new Vector3(.8f * percent, redBar.localScale.y, redBar.localScale.z);
    }

    IEnumerator DelayedDeath(float delay) {
        yield return new WaitForSeconds(delay);
        Death();
    }
}
