﻿using System.Collections.Generic;
using System.Net;
using NSubstitute;
using NUnit.Framework;
using RestSharp;

namespace PusherServer.Tests.UnitTests
{
    [TestFixture]
    public class When_using_GetResult_to_deserialise_a_rest_response
    {
        readonly DefaultDeserializer _defaultDeserializer = new DefaultDeserializer();

        [Test]
        public void GetResult_should_gracefully_handle_a_deserialisation_exception()
        {
            var _stubRestResponse = Substitute.For<IRestResponse>();
            _stubRestResponse.Content.Returns("not json");
            _stubRestResponse.StatusCode.Returns(HttpStatusCode.BadRequest);

            var getResult = new GetResult<object>(_stubRestResponse, _defaultDeserializer);

            Assert.AreEqual(HttpStatusCode.BadRequest, getResult.StatusCode);
            StringAssert.IsMatch("not json", getResult.OriginalContent);
            StringAssert.IsMatch("The HTTP response could not be deserialized to the expected type. The following exception occurred: ", getResult.Body);
            Assert.AreEqual(_stubRestResponse, getResult.Response);
        }

        [Test]
        public void GetResult_should_deserialise_a_valid_json_response()
        {
            var _stubRestResponse = Substitute.For<IRestResponse>();
            _stubRestResponse.Content.Returns("[\"string1\", \"string2\", \"string3\"]");

            var getResult = new GetResult<List<string>>(_stubRestResponse, _defaultDeserializer);

            Assert.AreNotEqual(HttpStatusCode.BadRequest, getResult.StatusCode);
            Assert.AreEqual(3, getResult.Data.Count);
            StringAssert.IsMatch("string2", getResult.Data[1]);
            Assert.AreEqual(_stubRestResponse, getResult.Response);
        }
    }
}
