var img = new Image();
img.src = src = "Data/cards.svg";

$(function() {
    // globals
    var timer = null;
    var scenerTime = null;
    var userId = -1;
    var displayName = undefined;
    var roomName = undefined;

    var isHost = false;

    var overallPoints = 0;
    var points = 0;
    var hasVoted = false;
    var currentGame = undefined;
    var currentScene = 0;
    var currentSceneDuration = -1;
    var currentGameObject = undefined;

    var sharePointsAvailable = 0;

    var game = undefined;
    var duplicate_card = 0;

    const messageTypes = Object.freeze({
        "LoginPacket": 0,
        "LoginPacketAnswer": 1,
        "Message": 2,
        "Scoreboard": 3,
        "ChangeScene": 4,
        "GambleSet": 5,
        "GambleResult": 6,
        "ShareSet": 7,
        "ShareResult": 8,
        "WaitingGamesList": 9,
        "WaitingVote": 10,
        "WaitingVoteSelection": 11,
        "UpdatePlayer": 12,
        "UpdatePlayerList": 13,
        "SongGuessingSongPacket": 14,
        "SongGuessingAnswerPacket": 15,
        "SongGuessingIsHostPacket": 16
    });
    const sceneTypes = Object.freeze({ "Waiting": 0, "Gamble": 1, "Share": 2, "Scoreboard": 3,"Game": 4});
    var sceneStatesNames = ['Warteraum', 'Gamble', 'Schlücke teilen', 'Punktestand', 'Game'];
    const gameTypes = Object.freeze({ "HorseGame": 0, "SongGuesser": 1});
    var gameNames = ["Pferderennen", "Lieder Raten"];

    var nameCookie = Cookies.get("name");
    var roomCookie = Cookies.get("room");
    var playerTable = null;

    if (roomCookie == undefined || nameCookie == undefined) {
        window.location.href = "Login.html";
        return;
    } else {
        displayName = nameCookie;
        roomName = roomCookie;
    }

    game = $.connection.gameHub;
    game.client.post = function(messageType, messageData) {
        //alert(messageType + "," + messageData);
        if (userId === -1) {
            if (messageType === messageTypes.LoginPacketAnswer) {
                var obj1 = jQuery.parseJSON(messageData);
                if (obj1["PlayerId"] === -1) {
                    Cookies.remove("name");
                    Cookies.remove("room");
                    window.location.href = "Login.html";
                } else {
                    userId = obj1["PlayerId"];
                    displayName = obj1["DisplayName"];
                    $(".player-name").html(displayName);
                    // Eingeloggt;

                    sceneTimer = window.setInterval(updateSceneTimer, 1000);
                }
            } else {
                alert("Unknown packet while not logged" + messageData);
            }
        } else {
            // Eingeloggt
            if (messageType === messageTypes.Message) {
                var obj5 = jQuery.parseJSON(messageData);
                displaySmallMessage(obj5["Message"]);
                return;
            } else if (messageType === messageTypes.ChangeScene) {
                var obj3 = jQuery.parseJSON(messageData);
                var cs = obj3["SceneType"];
                currentSceneDuration = obj3["SceneDuration"];
                currentGame = obj3["GameType"];
                changeVisualSceneState(cs);
                changeScene(cs);
                return;
            }
            else if (messageType === messageTypes.UpdatePlayerList) {
                var obj2 = jQuery.parseJSON(messageData);
                playerTable = obj2;
                return;
            }
            else if (messageType === messageTypes.UpdatePlayer) {
                var obj9 = jQuery.parseJSON(messageData);
                points = obj9["Points"];
                overallPoints = obj9["OverallPoints"];
                $(".player-points-value").html(points);
                return;
            }

            // Auswertung nach Scene
            if (currentScene === sceneTypes.Game) {
                currentGameObject.PacketReceived(messageType, messageData);
            }
            else if (currentScene === sceneTypes.Waiting) {
                if (messageType === messageTypes.WaitingGamesList) {
                    $(".game-vote-list").html("");
                    var obj7 = jQuery.parseJSON(messageData);
                    $.each(obj7["GamesList"],
                        function(i, item) {
                            var n2 = "game-vote" + item["Id"];

                            var ele = "<div class='game-vote' id='" +
                                n2 +
                                "'>" +
                                "<div class='game-vote-image' style=\"background-image: url('Data/" +
                                item["Image"] +
                                "');\"><div class='game-vote-name'>" +
                                item["Name"] +
                                "</div></div></div>";
                            $(".game-vote-list").append(ele);
                            $("#" + n2).data("game_id", item["Id"]);
                            $("#" + n2).click(function() {
                                if (hasVoted)
                                    return;

                                hasVoted = true;
                                var id = $(this).data("game_id");
                                $("#game-vote" + id).append("<img src='Data/ok.png' class='game-vote-icon' />");
                                var np = new Object();
                                np.GameId = id;
                                var data = JSON.stringify(np);
                                game.server.post(messageTypes.WaitingVote, data);
                            });
                        });
                } else if (messageType === messageTypes.WaitingVoteSelection) {
                    var obj8 = jQuery.parseJSON(messageData);
                    $(".game-vote-count").html("(" + obj8["Count"] + " von " + obj8["OverallCount"] + ")");

                    var id = obj8["Game"];

                    // Lösche alte Selektion
                    $(".game-vote-selected").removeClass("game-vote-selected");
                    if (id !== -1) {
                        $("#game-vote" + id).addClass("game-vote-selected");
                    }
                }

            } else if (currentScene === sceneTypes.Scoreboard) {
                if (messageType === messageTypes.Scoreboard) {
                    var obj2 = jQuery.parseJSON(messageData);
                    $(".player-highscore-table tbody > tr").remove();
                    $.each(obj2["Players"],
                        function(i, item) {
                            var ext = "";
                            var playerExt = "";
                            if (item["PlayerId"] === userId) {
                                playerExt = " class='highscore-own-name'";
                            }
                            if (item["Movement"] === 0) {
                                ext = "<img class='highscore-icon' src='Data/down.png'/>";
                            }
                            else if (item["Movement"] === 1) {
                                ext = "<img class='highscore-icon' src='Data/neutral.png'/>";
                            }
                            else if (item["Movement"] === 2) {
                                ext = "<img class='highscore-icon' src='Data/up.png'/>";
                            }
                            var ele = "<tr><td>" + ext +
                                (i + 1) +
                                "</td>" +
                                "<td" + playerExt + ">" +
                                item["DisplayName"] +
                                "</td><td>" +
                                item["OverallPoints"] +
                                "</td><td>" +
                                item["Points"] +
                                "</td><td>" + item["Drunk"] + "</td></tr>";
                            $(".player-highscore-table tbody").append(ele);
                        });
                }

            } else if (currentScene === sceneTypes.Gamble) {
                if (messageType === messageTypes.GambleResult) {
                    $(".scene-manager").children().hide();
                    $("#duplicate-drinks-results-scene").show();

                    var obj4 = jQuery.parseJSON(messageData);
                    var isBlack = obj4["black"];
                    if (isBlack) {
                        $("#mini-card-black").show();
                        $("#mini-card-red").hide();
                    } else {
                        $("#mini-card-black").hide();
                        $("#mini-card-red").show();
                    }

                    cardLoader();
                    $.each(obj4["States"],
                        function(i, item) {
                            if (item["PlayerId"] === userId) {
                                if ((isBlack && item["AmountBlack"] > 0) || (!isBlack && item["AmountRed"] > 0)) {
                                    $("#duplicate-results-desc")
                                        .html("Du hast gewonnen <div class='gamble-winner'>+ 3</div>");
                                } else if (item["AmountBlack"] === 0 && item["AmountRed"] === 0) {
                                    $("#duplicate-results-desc").html("Du hast nicht mitgespielt.");
                                } else {
                                    $("#duplicate-results-desc")
                                        .html("Du hast verloren, du noob <div class='gamble-looser'>- 3</div>");
                                }
                            } else {
                                if ((isBlack && item["AmountBlack"] > 0) || (!isBlack && item["AmountRed"] > 0)) {
                                    $("#duplicate-other-results")
                                        .append("<div class='duplicate-result-entry duplicate-results-won'>" +
                                            item["PlayerName"] +
                                            " + 3</div> ");
                                } else if (item["AmountBlack"] === 0 && item["AmountRed"] === 0) {

                                } else {
                                    $("#duplicate-other-results")
                                        .append("<div class='duplicate-result-entry duplicate-results-lost'>" +
                                            item["PlayerName"] +
                                            " - 3 </div> ");
                                }
                            }
                        });
                }
            } else if (currentScene === sceneTypes.Share) {
                if (messageType === messageTypes.ShareResult) {
                    displaySmallMessage(messageData);
                    var obj6 = jQuery.parseJSON(messageData);
                    $("#share-title").html("Schlücke Verteilen - Ergebnisse");
                    $("#share-description").hide();
                    $("#share-drinks-scene").show();
                    $(".share-player-list").html("");
                    $.each(obj6["Data"],
                        function(i, item) {
                            var append = "";
                            if (item["DrinkValue"] > 0) {
                                if (item["PlayerId"] === userId)
                                    append = "share-player-main";

                                var n2 = "share-player" + item["PlayerId"];
                                var ele = "<div class='share-player " +
                                    append +
                                    "' id='" +
                                    n2 +
                                    "'><div class='share-player-name'>" +
                                    item["DisplayName"] +
                                    "</div><div class='share-player-count'>" +
                                    item["DrinkValue"] +
                                    "</div><div class='share-beer-icon'></div></div>";
                                $(".share-player-list").append(ele);
                            }
                        });
                }
            } else if (currentScene === sceneTypes.SongGuesser) {
                if (messageType === messageTypes.SongGuessingSongPacket) {
                    var songGuessingPacket = jQuery.parseJSON(messageData);
                    var songLink = songGuessingPacket["SongLink"];

                    if (isHost) {
                        $("#iframe-dj").html(
                            "<iframe id=\"song-guesser-dj-link\" width=\"560\" height=\"315\" src=\"https://www.youtube.com/embed/" +
                            songLink +
                            "?autoplay=1\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe>");
                    } else {
                        $("#iframe-guesser").html(
                            "<iframe id=\"song-guesser-guesser-link\" width=\"1\" height=\"1\" src=\"https://www.youtube.com/embed/" +
                            songLink +
                            "?autoplay=1\" frameborder=\"0\" allow=\"accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe>");
                    }


                } else if (messageType === messageTypes.SongGuessingIsHostPacket) {
                    var isHostPacket = jQuery.parseJSON(messageData);
                    var isHost = isHostPacket["IsSongProvider"];

                    if (isHost) {
                        $("#song-dj").show();
                        $("#scene-description").html("Du bist DJ");

                    } else {
                        $("#song-guesser").show();
                        $("#scene-description").html("Du bist ein Ratefuchs");
                    }
                }
            }

        }
    };

    game.client.addNewMessageToPage = function(message) {
        // Add the message to the page. 
        displaySmallMessage(message);
    };


    $.connection.hub.start().done(function() {
        if (roomName === undefined || displayName === undefined)
            return;
        var loginPacket = new Object();
        loginPacket.Room = roomName;
        loginPacket.DisplayName = displayName;
        var data = JSON.stringify(loginPacket);
        game.server.post(messageTypes.LoginPacket, data);
    });

    function changeScene(state) {
        if (currentScene === sceneTypes.Game) {
            currentGameObject.GameEnded();
            currentGame = undefined;
            currentGameObject = undefined;
        }
        currentScene = state;


        $(".scene-manager").show();
        $(".scene-manager").children().hide();
        if (state === sceneTypes.Waiting) {
            $("#waiting-scene").show();
            hasVoted = false;
        }
        if (state === sceneTypes.Scoreboard) {
            $("#scoreboard-scene").show();
        }
        if (state === sceneTypes.Gamble) {
            if (points < 3) {
                $(".scene-description").text("Du hast nicht genügend Einsätze zum gamblen");
            }
            duplicate_card = 0;
            hideCoins();

            $("#duplicate-drinks-scene").show();
            cardLoader();
        } else if (state === sceneTypes.Share) {
            $("#share-drinks-scene").show();
            $(".share-player-list").html("");
            $.each(playerTable["Players"],
                function (i, item) {

                    if (item["PlayerId"] !== userId) {
                        var n2 = "share-player" + item["PlayerId"];
                        var ele = "<div class='share-player' id='" +
                            n2 +
                            "'><div class='share-player-name'>" +
                            item["DisplayName"] +
                            "</div><div class='share-player-count'>0</div></div>";
                        $(".share-player-list").append(ele);
                        $("#" + n2).data("val", "0");
                        $("#" + n2).data("id", item["PlayerId"]);
                        // Beschränke Updates, sammle alle Änderrungen zsm und schicke erst nach einer Sekunde
                        $("#" + n2).on('mousewheel',
                            function (event) {
                                var locked = $(this).data("locked");
                                if (!locked)
                                    $(this).data("locked", true);
                                var v = parseInt($(this).data("val"));
                                var id = $(this).data("id");
                                var g = 0;
                                if (event.deltaY > 0) {
                                    if (v < 5 && sharePointsAvailable > 0) {

                                        g = -1;
                                        v += 1;
                                    }
                                } else {
                                    if (v > 0) {
                                        v -= 1;
                                        g = 1;
                                    }

                                }
                                $(this).data("val", v);
                                sharePointsAvailable += g;
                                $(".share-player-amount-total").html(sharePointsAvailable);

                                $("#share-player" + id + " .share-player-count").html(v);

                                if (locked)
                                    return;
                                locked = true;
                                setTimeout(function () {
                                    var v2 = $("#share-player" + id).data("val");
                                    var np = new Object();
                                    np.PlayerId = id;
                                    np.Amount = v2;
                                    var data = JSON.stringify(np);
                                    game.server.post(messageTypes.ShareSet, data);
                                    $("#share-player" + id).data("locked", false);
                                },
                                    1000);
                            });
                    } else {
                        sharePointsAvailable = points;
                        $(".share-player-amount-total").html(sharePointsAvailable);
                    }
                });
        }
        else if (state === sceneTypes.Game) {
            $(".scene-manager").hide();
            $(".game-body").show();

            if (currentGame === gameTypes.HorseGame) {
                currentGameObject = new HorseGame($(".game-body"), game, userId, displayName, playerTable );
                currentGameObject.Init();
            }
            else if (currentGame === gameTypes.SongGuesser) {

            }

        }
    }

    $("#card-black").click(function() {
        if (duplicate_card === 2) {
            hideCoins();
        }
        if (points >= 3 && duplicate_card !== 1) {

            duplicate_card = 1;
            $("#card-black .card-coins").show();
            sendGambleSet(3, 0);
        }
    });

    $("#card-red").click(function() {
        if (duplicate_card === 1) {
            hideCoins();
        }
        if (points >= 3 && duplicate_card !== 2) {
            duplicate_card = 2;
            $("#card-red .card-coins").show();
            sendGambleSet(0, 3);
        }
    });

    $("#sendSongButton").click(function() {
        var songPacket = new Object();
        songPacket.SongLink = $("#songLink").val();
        songPacket.SongTitle = $("#songTitle").val();
        songPacket.SongArtist = $("#songArtist").val();
        var data = JSON.stringify(songPacket);

        game.server.post(messageTypes.SongGuessingSongPacket, data);
    });

    $("#sendAnswerButton").click(function() {
        var songAnswerPacket = new Object();
        songAnswerPacket.SongTitle = $("#songGuesserTitle").val();
        songAnswerPacket.SongArtist = $("#songGuesserArtist").val();
        var data = JSON.stringify(songAnswerPacket);

        game.server.post(messageTypes.SongGuessingAnswerPacket, data);
    });

    function sendGambleSet(black, red) {
        var gambleSetPacket = new Object();
        gambleSetPacket.AmountRed = black;
        gambleSetPacket.AmountBlack = red;
        var data = JSON.stringify(gambleSetPacket);
        game.server.post(messageTypes.GambleSet, data);
    }

    function hideCoins() {
        $("#card-red .card-coins").hide();
        $("#card-black .card-coins").hide();
    }

    function cardLoader() {
        $(".card").each(function(i, obj) {
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
                ctx.drawImage(img,
                    x + border,
                    y + border,
                    width - border,
                    height - border,
                    0,
                    0,
                    (obj).width,
                    (obj).height);

                $(obj).data("card-loaded", "true");
            }
        });
    }

    function displaySmallMessage(message) {
        $("#message-small").text(message);
        $("#message-small").show().fadeOut(2000);
    }

    function displayBigMessage(message) {
        $("#message-big").text(message);
        $("#message-big").show();
        $("#message-big").delay(4000).fadeOut(200);

    }

    function changeVisualSceneState(state) {
        if (state === sceneTypes.Scoreboard) {
            // Zeige keine Bieranimation
            return;
        }
        $(".scene-manager").fadeOut(0);
        if (currentGame === -1) {
            displayBigMessage(sceneStatesNames[state]);
        } else {
            displayBigMessage(gameNames[currentGame]);
        }
        

        var $top = $('.animation-beer-top');
        $top.css('top', '100%');
        $top.show().animate({ top: '0%' },
            2000,
            function() {
                $(".animation-beer-top").delay(2000).fadeOut(100);
            });

        var $body = $('.animation-beer-body');
        $body.css('height', '0%');
        $body.css('top', '100%');
        $body.show().animate({ height: '100%', top: '12%' },
            2000,
            function() {
                $body.delay(2000).fadeOut(100);
                $(".scene-manager").show();
            });


    }

    function updateSceneTimer() {
        if (currentSceneDuration >= 0) {
            currentSceneDuration -= 1;
        }
        if (currentSceneDuration > 0) {

            $(".scene-timer").show();
            $(".scene-timer").text(currentSceneDuration);
        } else {
            $(".scene-timer").hide();
        }
    }
});

class GamePrototype
{
    constructor(parentElement, game, playerId, playerName, playerTable) {
        this.ParentElement = parentElement;
        this.Game = game;
        this.PlayerId = playerId;
        this.PlayerName = playerName;
        this.PlayerTable = playerTable;
    }

    Init() {
    }

    PacketReceived(messageType, messageData) {

    }

    GameEnded() {

    }

    SendPacket(messageType, data) {
        this.game.server.post(messageType, data);
    }
}