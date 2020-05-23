using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public void MakeTurn()
    {
        StartCoroutine(EnemyTurn(GameManagerScript.instance.enemyHandCards));
    }

    IEnumerator EnemyTurn(List<CardControllerScript> cards)
    {
        yield return new WaitForSeconds(1);
        GameManagerScript gameManager = GameManagerScript.instance;

        int count = Random.Range(0, cards.Count + 1);

        for(int i = 0; i < count; i++) //выставление карт соперником для проверки
        {
            if (gameManager.enemyFieldCards.Count > 4/*баг с выкладыванием бльшего кол-ва карт*/ || GameManagerScript.instance.enemyEnergy == 0
                || gameManager.enemyHandCards.Count == 0)
                break;

            List<CardControllerScript> cardsList = cards.FindAll(x => gameManager.enemyEnergy >= x.thisCard.cost && !x.thisCard.isSpell); //карты с подходящей ценой в руке

            if(cardsList.Count == 0)
                break;

            cardsList[0].GetComponent<CardMovementScript>().MoveToField(gameManager.enemyField);

            yield  return new WaitForSeconds(0.51f);

            cardsList[0].transform.SetParent(gameManager.enemyField);

            cardsList[0].OnCast();
        }

        yield  return new WaitForSeconds(1);

        while(gameManager.enemyFieldCards.Exists(x => x.thisCard.canAttack)) // атака соперником для проверки
        {
            var activeCard = gameManager.enemyFieldCards.FindAll(x => x.thisCard.canAttack)[0];
            bool hasProvocation  = gameManager.playerFieldCards.Exists(x => x.thisCard.isProvocation);

            if(hasProvocation || Random.Range(0, 2) == 0 && gameManager.playerFieldCards.Count > 0)
            {
                CardControllerScript enemy;

                if(hasProvocation)
                    enemy = gameManager.playerFieldCards.Find(x => x.thisCard.isProvocation);
                else
                    enemy = gameManager.playerFieldCards[Random.Range(0, gameManager.playerFieldCards.Count)];

                //Debug.Log(activeCard.SelfCard.Name);

                activeCard.thisCard.canAttack = false;

                activeCard.movement.MoveToTurget(enemy.transform);
                yield return new WaitForSeconds(.75f);

                gameManager.CardsFight(enemy, activeCard);
            }
            else
            {
                activeCard.thisCard.canAttack = false;

                activeCard.GetComponent<CardMovementScript>().MoveToTurget(gameManager.playerHero.transform);
                yield return new WaitForSeconds(.75f);

                gameManager.DamageHero(activeCard, false);
            }
            yield return new WaitForSeconds(.2f);
        }
        yield return new WaitForSeconds(1);
        gameManager.ChangeTurn();
    }





    /*void CastSpell(CardController card)
    {
        switch (((SpellCard)card.Card).SpellTarget)
        {
            case SpellCard.TargetType.NO_TARGET:

                switch (((SpellCard)card.Card).Spell)
                {
                    case SpellCard.SpellType.HEAL_ALLY_FIELD_CARDS:

                        if (GameManagerScr.Instance.EnemyFieldCards.Count > 0)
                            StartCoroutine(CastCard(card));

                        break;

                    case SpellCard.SpellType.DAMAGE_ENEMY_FIELD_CARDS:

                        if (GameManagerScr.Instance.PlayerFieldCards.Count > 0)
                            StartCoroutine(CastCard(card));

                        break;

                    case SpellCard.SpellType.HEAL_ALLY_HERO:
                        StartCoroutine(CastCard(card));
                        break;

                    case SpellCard.SpellType.DAMAGE_ENEMY_HERO:
                        StartCoroutine(CastCard(card));
                        break;
                }

                break;

            case SpellCard.TargetType.ALLY_CARD_TARGET:

                if (GameManagerScr.Instance.EnemyFieldCards.Count > 0)
                    StartCoroutine(CastCard(card,
                        GameManagerScr.Instance.EnemyFieldCards[Random.Range(0, GameManagerScr.Instance.EnemyFieldCards.Count)]));

                break;

            case SpellCard.TargetType.ENEMY_CARD_TARGET:

                if (GameManagerScr.Instance.PlayerFieldCards.Count > 0)
                    StartCoroutine(CastCard(card,
                        GameManagerScr.Instance.PlayerFieldCards[Random.Range(0, GameManagerScr.Instance.PlayerFieldCards.Count)]));

                break;
        }
    }

    IEnumerator CastCard(CardController spell, CardController target = null)
    {
        if (((SpellCard)spell.Card).SpellTarget == SpellCard.TargetType.NO_TARGET)
        {
            spell.GetComponent<CardMovementScr>().MoveToField(GameManagerScr.Instance.EnemyField);
            yield return new WaitForSeconds(.51f);

            spell.OnCast();
        }
        else
        {
            spell.Info.ShowCardInfo();
            spell.GetComponent<CardMovementScr>().MoveToTarget(target.transform);
            yield return new WaitForSeconds(.51f);

            GameManagerScr.Instance.EnemyHandCards.Remove(spell);
            GameManagerScr.Instance.EnemyFieldCards.Add(spell);
            GameManagerScr.Instance.ReduceMana(false, spell.Card.Manacost);

            spell.Card.IsPlaced = true;

            spell.UseSpell(target);
        }

        string targetStr = target == null ? "no_target" : target.Card.Name;
        Debug.Log("AI spell cast: " + (spell.Card).Name + " target: " + targetStr);
    }*/
}
