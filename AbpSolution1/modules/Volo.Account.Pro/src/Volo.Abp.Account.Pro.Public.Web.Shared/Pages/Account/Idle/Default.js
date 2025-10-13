(function () {
	var idleTracker = null;
	var signOutText = $('#AccountIdleSignOutNow').text();
	var accountIdleModal = new bootstrap.Modal('#AccountIdleModal');
	var signOutTimer = null;
	var timeout = $('#AccountIdleTimeoutMinutes').val();

	var resetIdleTracker = function () {
		$('#AccountIdleSignOutNow').text(signOutText);
		if (signOutTimer != null) {
			clearInterval(signOutTimer);
		}
		accountIdleModal.hide();
		idleTracker.resume();
		idleTracker.resetTimer();
	};

	idleTracker = abp.idleTracker.create({
		timeout: timeout * 60 * 1000, //minutes to milliseconds
		onIdleCallback: function (state) {
			if (state.idle) {
				accountIdleModal.show();
				idleTracker.pause();
				var countDown = 60;
				signOutTimer = setInterval(function () {
					$('#AccountIdleSignOutNow').text(signOutText + ' ( ' + countDown + ' )');
					countDown--;
					if (countDown <= 0) {
						resetIdleTracker();
						$('#AccountIdleSignOutNow').click();
					}
				}, 1000);
			}
		},
		onStorageCallback: function (state) {
			if (!state.idle) {
				resetIdleTracker();
			}
		}
	});

	idleTracker.start();
	idleTracker.syncState();

	$('#AccountIdleSignOutNow').click(function () {
		idleTracker.end();
		location.href = abp.appPath + $('#AccountIdleLogoutUrl').val();
	});

	$('#AccountIdleStaySignedIn').click(function () {
		resetIdleTracker();
	});
})();
