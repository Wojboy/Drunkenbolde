var img = new Image();
img.src = src = "Data/cards.svg";

$(function () {
    // globals
    var timer = null;
    var logged = false;
    var userId = -1;
    var displayName = undefined;
    var overallPoints = 0;
    var points = 0;

    var currentGame = undefined;
    var currentLobbyState = 0;

    var game = undefined;
    var password = undefined;

    var duplicate_card = 0;

    const messageTypes = Object.freeze({ "MessageSmall": 0, "MessageBig": 1, "Leaderboard": 2, "Login": 3, "ChangeState": 4, "LobbyUpdate": 5});
    const lobbyState = Object.freeze({ "Waiting": 0, "ShareDrinks": 1, "ShareDrinksResults": 2, "DuplicateDrinks": 3, "DuplicateDrinksResults": 4, "InGame": 5, "GameEnded" : 6 });
    var lobbyStatesNames = ['Warteraum', 'Schlücke austeilen', 'Schlücke Ergebnisse', 'Gamble', 'Gamble Ergebnisse' , 'Spiel: ', 'Spiel vorbei'];
    var gameNames = ["Pferderennen"];

    var loginCookie = Cookies.get("login");
    if (loginCookie == undefined) {
        window.location.href = "Login.html";
        return;
    } else {
        password = loginCookie;
    }

    var game = $.connection.gameHub;
    game.client.broadcastMessage = function (messageType, messageData) {
        if (!logged) {
            if (messageType === messageTypes.Login) {
                if (messageData === "-1") {
                    Cookies.delete("login");
                    window.location.href = "Login.html";
                } else {
                    userId = messageData;
                    logged = true;
                }
            }
        } else {
            // Eingeloggt

            if (messageType === messageTypes.MessageSmall) {
                displaySmallMessage(messageData);
            }
            else if (messageType === messageTypes.MessageBig) {
                displayBigMessage(messageData);
            }
            else if (messageType === messageTypes.Leaderboard) {
                var obj = jQuery.parseJSON(messageData);
                $(".player-highscore-table tbody > tr").remove();
                $.each(obj,
                    function (i, item) {
                        if (item["Id"] === userId) {
                            if (item["IsAdmin"]) {

                                $(".admin-menu").show();

                                $.each(gameNames, function (key, value) {
                                    $('#admin-game-select')
                                        .append($("<option></option>")
                                            .attr("value", key)
                                            .text(value));
                                });

                                timer = setInterval(adminPing, 1000);
                            }
                            displayName = item["DisplayName"];
                            points = item["Points"];
                            overallPoints = item["OverallPoints"];

                            $(".player-name").html(displayName);
                            $(".player-points-value").html(points);
                        }
                        var ele = "<tr><td>" +
                            (i + 1) + 
                            "</td><td>" +
                            item["DisplayName"] +
                            "</td><td>" +
                            item["OverallPoints"] +
                            "</td><td>" +
                            item["Points"] +
                            "</td></tr>";
                        $(".player-highscore-table tbody").append(ele);
                    });
            }
            else if (messageType === messageTypes.ChangeState) {
                currentLobbyState = messageData;
                changeVisualLobbyState(currentLobbyState);

                if (currentLobbyState !== lobbyState.InGame) {
                    changeScene(currentLobbyState);
                }
            }
            else if (messageType === messageTypes.LobbyUpdate) {
                if (currentLobbyState === lobbyState.DuplicateDrinksResults) {
                    var obj2 = jQuery.parseJSON(messageData);
                    var isBlack = obj2["black"];
                    if (isBlack) {
                        $("#mini-card-black").show();
                        $("#mini-card-red").hide();
                    } else {
                        $("#mini-card-black").hide();
                        $("#mini-card-red").show();
                    }

                    $.each(obj2["States"],
                        function (i, item) {
                            if (item["PlayerId"] === userId) {
                                if ((isBlack && item["AmountBlack"] > 0) || (!isBlack && item["AmountRed"] > 0)) {
                                    $("#duplicate-results-desc")
                                        .html("Du hast gewonnen <div style='color: mediumaquamarine;'>+ 3</div>");
                                } else if (item["AmountBlack"] === 0 && item["AmountRed"] === 0) {
                                    $("#duplicate-results-desc").html("Du hast nicht mitgespielt.");
                                } else {
                                    $("#duplicate-results-desc")
                                        .html("Du hast verloren, du noob <div style='color: indianred;'>- 3</div>");
                                }
                            } else {
                                if ((isBlack && item["AmountBlack"] > 0) || (!isBlack && item["AmountRed"] > 0)) {
                                    $("#duplicate-other-results").append("<div class='duplicate-result-entry duplicate-results-won'>" + item["PlayerName"] + " + 3</div> ");
                                } else if (item["AmountBlack"] === 0 && item["AmountRed"] === 0) {

                                } else {
                                    $("#duplicate-other-results").append("<div class='duplicate-result-entry duplicate-results-lost'>" + item["PlayerName"] + " - 3 </div> ");
                                }
                            }
                        });
                }
                else if (currentLobbyState === lobbyState.ShareDrinks) {

                }
            }
        }
    };

    game.client.addNewMessageToPage = function (message) {
        // Add the message to the page. 
        displaySmallMessage(message);
    };


    $.connection.hub.start().done(function () {
        if (password == undefined)
            return;

        game.server.login(password);
    });
    function changeScene(state) {
        $(".scene-manager").children().hide();

        if (state === lobbyState.DuplicateDrinks) {
            if (points < 3) {
                $(".scene-description").text("Du hast nicht genügend Einsätze zum gamblen");
            }
            duplicate_card = 0;
            hideCoins();
            $("#duplicate-drinks-scene").show();
            cardLoader();
        }
        else if (state === lobbyState.DuplicateDrinksResults) {
            $("#duplicate-drinks-results-scene").show();
            cardLoader();
        }
        else if (state === lobbyState.ShareDrinks) {
            $("#share-drinks-scene").show();
        }
    }

    function cardLoader() {
        $(".card").each(function (i, obj) {
            if (!$(obj).data("card-loaded")) {

                var row = $(obj).data("cr");
                var column = $(obj).data("cc");
 
                var ctx = obj.getContext("2d");

                var width = 360;
                var height = 540;

                var y = (30 * (row + 1) + height * row);
                var x = (30 * (column + 1) + width * column);
                ctx.imageSmoothingEnabled = false;
                var border = 3;
                ctx.drawImage(img, x + border, y + border, width - border, height - border, 0, 0, (obj).width, (obj).height);

                $(obj).data("card-loaded", "true");
            }
        });
    }
    function adminPing() {
        game.server.ping();
    }
    function displaySmallMessage(message) {
        $("#message-small").text(message);
        $("#message-small").show().fadeOut(2000);
       
    }

    function changeVisualLobbyState(state) {
        $(".scene-manager").hide();
        displayBigMessage(lobbyStatesNames[state]);

        var $top = $('.animation-beer-top');
        $top.css('top', '100%');
        $top.show().animate({ top: '0%'}, 2000, function () {
            $(".animation-beer-top").delay(2000).fadeOut(100);
        });

        var $body = $('.animation-beer-body');
        $body.css('height', '0%');
        $body.css('top', '100%');
        $body.show().animate({ height: '100%', top: '12%' }, 2000, function () {
            $body.delay(2000).fadeOut(100);
            $(".scene-manager").show();
        });
        

    }

    function displayBigMessage(message) {
        $("#message-big").text(message);
        $("#message-big").show();
        $("#message-big").delay(4000).fadeOut(200);

    }

    $("#card-black").click(function () {
        if (duplicate_card === 2) {
            hideCoins();
        }
        if (points >= 3 && duplicate_card !== 1) {

            duplicate_card = 1;
            $("#card-black .card-coins").show();
            game.server.lobbyupdate(password, "3-0");
        }
    });

    $("#card-red").click(function () {
        if (duplicate_card === 1) {
            hideCoins();
        }
        if (points >= 3 && duplicate_card !== 2) {
            duplicate_card = 2;
            $("#card-red .card-coins").show();
            game.server.lobbyupdate(password, "0-3");
        }
    });

    function hideCoins() {
        $("#card-red .card-coins").hide();
        $("#card-black .card-coins").hide();
    }


});