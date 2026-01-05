using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public void OnAttack()
    {
        if (EscToggleOptions.GameIsPaused) return;
        Attack();
    }

    private Renderer playerRenderer;
    private void Awake()
    {
        playerRenderer = GetComponent<Renderer>();
    }

    private void Attack()
    {
        float delay = 0.15f;

        Debug.Log("Attack triggered.");
        // Might need to end up placing the attack logic in here.
        if (playerRenderer != null)
        {
            playerRenderer.material.color = Color.red;
            Debug.Log("\tColor changed to: Red");
            StartCoroutine(RevertPlayerColor(delay));
        }

        // TODO: Add damage logic. Probably needs some form of a weapons framework.
    }
    // Not necessary, but that way a visual indicator of attack being registered can be seen.
    private IEnumerator RevertPlayerColor(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerRenderer.material.color = Color.white;
        Debug.Log("\tColor changed to: White");
    }

}
