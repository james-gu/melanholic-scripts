using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_CrackableStats : Script_CharacterStats
{
    [SerializeField] private Sprite defaultImage;
    [SerializeField] private Sprite lowHealthImage;
    [SerializeField] private int lowHealthThreshold;
    [SerializeField] private SpriteRenderer graphics;
    
    public override int Hurt(int dmg, Script_HitBox hitBox)
    {
        dmg = Mathf.Clamp(dmg, 0, int.MaxValue);

        // reduce health
        currentHp -= dmg;
        currentHp = Mathf.Clamp(currentHp, 0, int.MaxValue);
        
        Debug.Log($"{transform.name} took damage {dmg}. currentHp: {currentHp}");
        
        if (currentHp == 0)
        {
            Die(Script_GameOverController.DeathTypes.Default);
        }
        else
        {
            // Show a different spirte depending on how hurt the Interactable is
            HandleHurtGraphics(currentHp);
        }

        return dmg;
    }
    
    protected override void Die(Script_GameOverController.DeathTypes deathType)
    {
        Debug.Log("**** CRACKABLE DIE() ****");
        gameObject.SetActive(false);
    }

    private void HandleHurtGraphics(int hp)
    {
        if (graphics == null)   return;
        
        if      (hp <= lowHealthThreshold)      graphics.sprite = lowHealthImage;
        else                                    graphics.sprite = defaultImage;
    }
}
