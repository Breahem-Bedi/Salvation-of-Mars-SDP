﻿using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CloneControllerTest
    {
        private GameObject Player;
        private GameObject Clone;
        private CloneController cc;

        [SetUp]
        public void Init()
        {
            // setup player object
            Player = new GameObject();
            Player.AddComponent<PlayerMovement>();
            Player.AddComponent<Rigidbody2D>();
            Player.AddComponent<CharacterController2D>();
            GameObject ceilingCheckP = new GameObject();
            GameObject floorCheckP = new GameObject();
            ceilingCheckP.transform.parent = Player.transform;
            floorCheckP.transform.parent = Player.transform;
            Player.GetComponent<CharacterController2D>().GroundCheck = floorCheckP.transform;
            Player.GetComponent<CharacterController2D>().CeilingCheck = ceilingCheckP.transform;
            Player.GetComponent<PlayerMovement>().controller = Player.GetComponent<CharacterController2D>();

            // setup clone object
            Clone = new GameObject();
            Clone.AddComponent<PlayerMovement>();
            Clone.AddComponent<Rigidbody2D>();
            Clone.AddComponent<CharacterController2D>();
            GameObject ceilingCheckC = new GameObject();
            GameObject floorCheckC = new GameObject();
            ceilingCheckC.transform.parent = Clone.transform;
            floorCheckC.transform.parent = Clone.transform;
            Clone.GetComponent<CharacterController2D>().GroundCheck = floorCheckC.transform;
            Clone.GetComponent<CharacterController2D>().CeilingCheck = ceilingCheckC.transform;
            Clone.GetComponent<PlayerMovement>().controller = Clone.GetComponent<CharacterController2D>();

            // setup clone controller
            GameObject cloneController = new GameObject();
            cloneController.AddComponent<CloneController>();
            cc = cloneController.GetComponent<CloneController>();
            cc.clone = Clone;
            cc.player = Player;
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.Destroy(Player);
            UnityEngine.Object.Destroy(Clone);
            UnityEngine.Object.Destroy(cc);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.

        // test that, when first loaded, player is active and clone is inactive
        [UnityTest]
        public IEnumerator CloneStartsDisabledAndPlayerStartsEnabledTest()
        {
            Assert.IsTrue(Player.activeSelf); // player is currently active
            Assert.IsFalse(Clone.activeSelf); // clone is currently inactive

            yield return null;
        }

        [UnityTest]
        public IEnumerator CloneActivatedFirstTime()
        {
            cc.cloneAbilityButtonPressed(); // trigger press of clone ability button

            // on trigger of clone ability first time, player should be frozen but active, and clone should be unfrozen and active
            Assert.IsTrue(Player.activeSelf); // player is currently active
            Assert.IsTrue(Clone.activeSelf); // clone is also currently active

            Assert.IsTrue(cc.pFrozen); // player is frozen
            Assert.IsFalse(cc.cFrozen); // clone is NOT frozen

            yield return null;
        }

        [UnityTest]
        public IEnumerator SwitchBackToPlayer()
        {
            // this tests that after creating the clone, the user can switch back to the player by pressing the clone ability button again.
            // the clone should remain active, but frozen. the player should be unfrozen

            cc.cloneAbilityButtonPressed(); // trigger press of clone ability button first time
            cc.cloneAbilityButtonPressed(); // trigger press of clone ability button again

            Assert.IsTrue(Player.activeSelf); // player is currently active
            Assert.IsTrue(Clone.activeSelf); // clone is currently active

            Assert.IsFalse(cc.pFrozen); // player is not frozen
            Assert.IsTrue(cc.cFrozen); // clone IS frozen

            yield return null;
        }

        [UnityTest]
        public IEnumerator SwitchBackToClone()
        {
            // this tests that after creating the clone and switching to the player, the user is able to switch BACK to the clone
            // the player and clone should remain active, but the player should be frozen and the clone should be frozen

            cc.cloneAbilityButtonPressed(); // trigger press of clone ability button first time
            cc.cloneAbilityButtonPressed(); // trigger press of clone ability button again to switch control to player
            cc.cloneAbilityButtonPressed(); // trigger press of clone ability button again, switch control to clone

            Assert.IsTrue(Player.activeSelf); // player is currently active
            Assert.IsTrue(Clone.activeSelf); // clone is currently active

            Assert.IsTrue(cc.pFrozen); // player is frozen
            Assert.IsFalse(cc.cFrozen); // clone is not frozen

            yield return null;
        }
    }
}