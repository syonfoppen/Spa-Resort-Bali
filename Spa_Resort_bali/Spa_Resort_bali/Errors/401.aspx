<% Response.StatusCode = 401 %>

<!DOCTYPE html>
<html xmlns='http://www.w3.org/1999/xhtml' lang='' xml:lang=''>
<head>
	<meta charset='utf-8' />
	<meta name='viewport' content='width=device-width, user-scalable=no' />
	<title></title>
    <link href="../Content/SRB_Error_Game.css" rel="stylesheet" />
</head>
<body>
	<canvas id='canvas'>
		HTML5 canvas appears to be unsupported in the current browser.<br />
		Please try updating or use a different browser.
	</canvas>
	<div id='status'>
		<div id='status-progress' style='display: none;' oncontextmenu='event.preventDefault();'><div id ='status-progress-inner'></div></div>
		<div id='status-indeterminate' style='display: none;' oncontextmenu='event.preventDefault();'>
			<div></div>
			<div></div>
			<div></div>
			<div></div>
			<div></div>
			<div></div>
			<div></div>
			<div></div>
		</div>
		<div id='status-notice' class='godot' style='display: none;'></div>
	</div>

<script src="../Content/SRB_Error_Game.js"></script>
	<script type='text/javascript'>//<![CDATA[

		var engine = new Engine;
		var setStatusMode;
		var setStatusNotice;
        
		(function() {

            const MAIN_PACK = '../Content/SRB_Error_Game.pck';
			const INDETERMINATE_STATUS_STEP_MS = 100;

			var canvas = document.getElementById('canvas');
			var statusProgress = document.getElementById('status-progress');
			var statusProgressInner = document.getElementById('status-progress-inner');
			var statusIndeterminate = document.getElementById('status-indeterminate');
			var statusNotice = document.getElementById('status-notice');

			var initializing = true;
			var statusMode = 'hidden';

			var animationCallbacks = [];
			function animate(time) {
				animationCallbacks.forEach(callback => callback(time));
				requestAnimationFrame(animate);
			}
			requestAnimationFrame(animate);

			function adjustCanvasDimensions() {
				canvas.width = innerWidth;
				canvas.height = innerHeight;
			}
			animationCallbacks.push(adjustCanvasDimensions);
			adjustCanvasDimensions();

			setStatusMode = function setStatusMode(mode) {

				if (statusMode === mode || !initializing)
					return;
				[statusProgress, statusIndeterminate, statusNotice].forEach(elem => {
					elem.style.display = 'none';
				});
				if (animateStatusIndeterminate in animationCallbacks) {
					animationCallbacks.erase(animateStatusIndeterminate);
				}
				switch (mode) {
					case 'progress':
						statusProgress.style.display = 'block';
						break;
					case 'indeterminate':
						statusIndeterminate.style.display = 'block';
						animationCallbacks.push(animateStatusIndeterminate);
						break;
					case 'notice':
						statusNotice.style.display = 'block';
						break;
					case 'hidden':
						break;
					default:
						throw new Error('Invalid status mode');
				}
				statusMode = mode;
			}

			function animateStatusIndeterminate(ms) {

				var i = Math.floor(ms / INDETERMINATE_STATUS_STEP_MS % 8);
				if (statusIndeterminate.children[i].style.borderTopColor == '') {
					Array.prototype.slice.call(statusIndeterminate.children).forEach(child => {
						child.style.borderTopColor = '';
					});
					statusIndeterminate.children[i].style.borderTopColor = '#dfdfdf';
				}
			}

			setStatusNotice = function setStatusNotice(text) {

				while (statusNotice.lastChild) {
					statusNotice.removeChild(statusNotice.lastChild);
				}
				var lines = text.split('\n');
				lines.forEach((line) => {
					statusNotice.appendChild(document.createTextNode(line));
					statusNotice.appendChild(document.createElement('br'));
				});
			};

			engine.setProgressFunc((current, total) => {

				if (total > 0) {
					statusProgressInner.style.width = current/total * 100 + '%';
					setStatusMode('progress');
					if (current === total) {
						// wait for progress bar animation
						setTimeout(() => {
							setStatusMode('indeterminate');
						}, 500);
					}
				} else {
					setStatusMode('indeterminate');
				}
			});

			function displayFailureNotice(err) {
				var msg = err.message || err;
				console.error(msg);
				setStatusNotice(msg);
				setStatusMode('notice');
				initializing = false;
			};

			if (!Engine.isWebGLAvailable()) {
				displayFailureNotice('WebGL not available');
			} else {
				setStatusMode('indeterminate');
				engine.setCanvas(canvas);
				engine.startGame(MAIN_PACK).then(() => {
					setStatusMode('hidden');
					initializing = false;
				}, displayFailureNotice);
			}
		})();
	//]]></script>
</body>
</html>