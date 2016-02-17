﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TMDbLib.Objects.Authentication;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Lists;
using TMDbLib.Objects.Movies;
using TMDbLibTests.Helpers;

namespace TMDbLibTests
{
    [TestClass]
    public class ClientListsTests
    {
        private TestConfig _config;
        private const string TestListId = "528349d419c2954bd21ca0a8";

        /// <summary>
        /// Run once, on every test
        /// </summary>
        [TestInitialize]
        public void Initiator()
        {
            _config = new TestConfig();
        }

        [TestMethod]
        public void TestList()
        {
            // Get list
            List list = _config.Client.GetList(TestListId).Result;

            Assert.IsNotNull(list);
            Assert.AreEqual(TestListId, list.Id);
            Assert.AreEqual(list.ItemCount, list.Items.Count);

            foreach (MovieResult movieResult in list.Items)
            {
                Assert.IsNotNull(movieResult);

                // Ensure all movies point to this list
                int page = 1;
                SearchContainer<ListResult> movieLists = _config.Client.GetMovieLists(movieResult.Id).Result;
                while (movieLists != null)
                {
                    // Check if the current result page contains the relevant list
                    if (movieLists.Results.Any(s => s.Id == TestListId))
                    {
                        movieLists = null;
                        continue;
                    }

                    // See if there is an other page we could try, if not the test fails
                    if (movieLists.Page < movieLists.TotalPages)
                        movieLists = _config.Client.GetMovieLists(movieResult.Id, ++page).Result;
                    else
                        Assert.Fail("Movie '{0}' was not linked to the test list", movieResult.Title);
                }
            }
        }
        
        [TestMethod]
        public void TestListIsMoviePresentFailure()
        {
            Assert.IsFalse(_config.Client.GetListIsMoviePresent(TestListId, IdHelper.Terminator).Result);
            _config.Client.SetSessionInformation(_config.UserSessionId, SessionType.UserSession);

            // Clear list
            Assert.IsTrue(_config.Client.ListClear(TestListId).Result);

            // Verify Avatar is not present
            Assert.IsFalse(_config.Client.GetListIsMoviePresent(TestListId, IdHelper.Avatar).Result);

            // Add Avatar
            Assert.IsTrue(_config.Client.ListAddMovie(TestListId, IdHelper.Avatar).Result);

            // Verify Avatar is present
            Assert.IsTrue(_config.Client.GetListIsMoviePresent(TestListId, IdHelper.Avatar).Result);
        }

        [TestMethod]
        public void TestListCreateAndDelete()
        {
            const string listName = "Test List 123";

            _config.Client.SetSessionInformation(_config.UserSessionId, SessionType.UserSession);
            string newListId = _config.Client.ListCreate(listName).Result;

            Assert.IsFalse(string.IsNullOrWhiteSpace(newListId));

            List newlyAddedList = _config.Client.GetList(newListId).Result;
            Assert.IsNotNull(newlyAddedList);
            Assert.AreEqual(listName, newlyAddedList.Name);
            Assert.AreEqual("", newlyAddedList.Description); // "" is the default value
            Assert.AreEqual("en", newlyAddedList.Iso_639_1); // en is the default value
            Assert.AreEqual(0, newlyAddedList.ItemCount);
            Assert.AreEqual(0, newlyAddedList.Items.Count);
            Assert.IsFalse(string.IsNullOrWhiteSpace(newlyAddedList.CreatedBy));

            Assert.IsTrue(_config.Client.ListDelete(newListId).Result);
        }

        [TestMethod]
        public void TestListDeleteFailure()
        {
            _config.Client.SetSessionInformation(_config.UserSessionId, SessionType.UserSession);

            // Try removing a list with an incorrect id
            Assert.IsFalse(_config.Client.ListDelete("bla").Result);
        }

        [TestMethod]
        public void TestListAddAndRemoveMovie()
        {
            _config.Client.SetSessionInformation(_config.UserSessionId, SessionType.UserSession);

            // Add a new movie to the list
            Assert.IsTrue(_config.Client.ListAddMovie(TestListId, IdHelper.EvanAlmighty).Result);

            // Try again, this time it should fail since the list already contains this movie
            Assert.IsFalse(_config.Client.ListAddMovie(TestListId, IdHelper.EvanAlmighty).Result);

            // Get list and check if the item was added
            List listAfterAdd = _config.Client.GetList(TestListId).Result;
            Assert.IsTrue(listAfterAdd.Items.Any(m => m.Id == IdHelper.EvanAlmighty));

            // Remove the previously added movie from the list
            Assert.IsTrue(_config.Client.ListRemoveMovie(TestListId, IdHelper.EvanAlmighty).Result);

            // Get list and check if the item was removed
            List listAfterRemove = _config.Client.GetList(TestListId).Result;
            Assert.IsFalse(listAfterRemove.Items.Any(m => m.Id == IdHelper.EvanAlmighty));
        }

        [TestMethod]
        public void TestListClear()
        {
            _config.Client.SetSessionInformation(_config.UserSessionId, SessionType.UserSession);

            // Add a new movie to the list
            Assert.IsTrue(_config.Client.ListAddMovie(TestListId, IdHelper.MadMaxFuryRoad).Result);

            // Get list and check if the item was added
            List listAfterAdd = _config.Client.GetList(TestListId).Result;
            Assert.IsTrue(listAfterAdd.Items.Any(m => m.Id == IdHelper.MadMaxFuryRoad));

            // Clear the list
            Assert.IsTrue(_config.Client.ListClear(TestListId).Result);

            // Get list and check that all items were removed
            List listAfterRemove = _config.Client.GetList(TestListId).Result;
            Assert.IsFalse(listAfterRemove.Items.Any());
        }
    }
}
