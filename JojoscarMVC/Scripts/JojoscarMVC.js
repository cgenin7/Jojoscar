$(
	function () {

	    var listener = new window.keypress.Listener();
	    listener.simple_combo("ctrl r", function () {
	        $("#GetNextResultId").click();
	    });
	}
)