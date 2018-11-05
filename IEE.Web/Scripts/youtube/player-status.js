var player;
function onYouTubeIframeAPIReady() {
	player = new YT.Player('player', {
		height: '390',
		width: '640',
		videoId: 'jU5i1WjRBhE',
		events: {
			'onReady': onPlayerReady,
			'onStateChange': onPlayerStateChange
		}
	});
}

function onPlayerReady(event) {
	window.setInterval(function () {
		//showCurrentTime()
	}, 5000);
}

function onPlayerStateChange(event) {
	switch (event.data) {
		case -1:
			alert('State: Unstarted');
			break;
		case 0:
			alert('State: Ended');
			break;
		case 1:
			alert('State: Playing');
			break;
		case 2:
			alert('State: Paused');
			break;
		case 3:
			alert('State: Buffering');
			break;
		case 5:
			alert('State: Video cued');
			break;
		default:
			alert('Default');
			break;
	}
}

function showCurrentTime() {
	if (player.getPlayerState() == 1) {
		alert(player.getCurrentTime());
	}
}