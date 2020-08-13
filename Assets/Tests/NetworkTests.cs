using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NetworkTests
    {
        [Test]
        public void GetScores()
        {
            // just checking it doesn't error out, not a full test
            //var result = Scores.GetScores("Level1");
            //Assert.Greater(result.Count, 3);
        }

        [Test]
        public void UploadScore()
        {
            // just checking it doesn't error out, not a full test
            Scores.UploadScores("unittesting_username","NetworkTestLevel",10);
        }
    }
}
