$(document).ready(function ()
{
    /*
    $(".demo-card-image").on("swipeleft", function () {

        $(this).addClass("rotate-right").delay(300).queue(function (next) {
            $(this).removeClass("rotate-right");
            $(this).addClass('rotate-back');
            next();
        }).delay(1000).queue(function (next) {
            $(this).removeClass("rotate-back");
            next();
        });
    });

    $(".demo-card-image").on("swiperight", function () {

        $(this).addClass("rotate-left").delay(300).queue(function (next) {
            $(this).removeClass("rotate-left");
            $(this).addClass('rotate-back');
            next();
        }).delay(1000).queue(function (next) {
            $(this).removeClass('rotate-back');
            next();
        });
    });
    */

    var canClick = true;

    function updateBars(element, data) {
        var goodPercentDisplay = (data.upvotePercent * 100).toFixed(0) + "%";
        var goodPercentWidth = (data.upvotePercent * 100).toFixed(4) + "%";

        var goodBar = $(element).parent().prevAll(".ratingcontainer").children(".goodratingbar");
        goodBar.text(goodPercentDisplay);
        goodBar.finish().animate({ width: goodPercentWidth }, 400);

        var badPercentDisplay = (data.downvotePercent * 100).toFixed(0) + "%";
        var badPercentWidth = (data.downvotePercent * 100).toFixed(4) + "%";

        var badBar = $(element).parent().prevAll(".ratingcontainer").children(".badratingbar");
        badBar.text(badPercentDisplay);
        badBar.finish().animate({ width: badPercentWidth }, 400);
    }

    var simplyOptions =
    {
        delay: 3000,
        align: "left"
    };

    $(".downbutton").on("click", function () {
        if (canClick)
        {
            canClick = false;
            var element = this;

            $.get("/rate/bad/" + $(element).data("item"))
            .done(function(data) {
                updateBars(element, data);
                $(element).text("-" + data.downvotes);
            })
            .fail(function(data) {
                if (data.status === 429)
                {
                    $.simplyToast("You can vote for " + $(element).data("item") + " again in " + data.responseText, 'info', simplyOptions);
                }
                else
                {
                    $.simplyToast("There was an internal server error. Please try again later.", 'error', simplyOptions);
                }
            })
            .always(function() {
                canClick = true;
            });
        }
    });


    $(".upbutton").on("click", function () {
        if (canClick)
        {
            canClick = false;
            var element = this;

            $.get("/rate/good/" + $(element).data("item"))
            .done(function (data) {
                updateBars(element, data);
                $(element).text("+" + data.upvotes);
            })
            .fail(function (data) {
                if (data.status === 429)
                {
                    $.simplyToast("You can vote for " + $(element).data("item") + " again in " + data.responseText, 'info', simplyOptions);
                }
                else
                {
                    $.simplyToast("There was an internal server error. Please try again later.", 'error', simplyOptions);
                }
            })
            .always(function () {
                canClick = true;
            });
        }
    });
});