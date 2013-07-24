<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApexSound.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.ApexSound" %>
<asp:placeholder id="soundButton" runat="server" visible="false">
<div class="module">
    <h2>Call Recording</h2>
    <table class="editForm" width="424">
        <tr><td id="apex-call-container"></td></tr>
    </table>
</div>
<script type="text/javascript">
    function createAudioPlayer(el, width, height, url, callID) {
        $(el).append(
            "<object id='mPlayer-" + callID + "' width='" + width + "' height='" + height + "' classid='CLSID:22d6f312-b0f6-11d0-94ab-0080c74c7e95'" +
            "codebase='http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=5,1,52,701' standby='Loading Microsoft Windows Media Player components...' type='application/x-oleobject'>" +
            "<param name='fileName' value='" + url + "'><param name='animationatStart' value='true'><param name='transparentatStart' value='true'><param name='autoStart' value='false'> <param name='showControls' value='true'><param name='ShowAudioControls' value='true'><param name='ShowStatusBar' value='true'><param name='loop' value='false'>" +
            "<embed type='application/x-mplayer2' pluginspage='http://microsoft.com/windows/mediaplayer/en/download/' name='mediaPlayer' displaysize='4' autosize='-1' bgcolor='darkblue'  showcontrols='1' showtracker='1' showdisplay='0' showstatusbar='1' videoborder3d='-1'" +
            " width='" + width + "' height='" + height + "' src='" + url + "' autostart='0' designtimesp='5311' loop='0'></object>");
        return $(el);
    }

    $(document).ready(function() {
        createAudioPlayer("#apex-call-container", 300, 70, 'http://203.153.53.212:2014/download.aspx?phone=<%=Request["phone"]%>', "1");
    });
</script>
</asp:placeholder>
<asp:placeholder id="oldSoundButton" runat="server" visible="false">
<div class="module">
<script type="text/javascript">
    $(document).ready(function () {
        $('#jp_container_1').hide();
        $("#jquery_jplayer_1").jPlayer(
        {
            ready: function () {
                $(this).jPlayer("setMedia", {
                    mp3: 'http://203.153.53.212:2014/download.aspx?phone=<%=Request["phone"]%>'
                });
            },
            canplay: function () {
                $("#jp_container_1").show();
            },
            errorAlert: true,
            swfPath: "/js",
            supplied: "mp3"
        });
    });
</script>
<div id="jquery_jplayer_1" class="jp-jplayer"></div>
	  <div id="jp_container_1" class="jp-audio">
		<div class="jp-type-single">
		  <div class="jp-gui jp-interface">
			<ul class="jp-controls">
			  <li><a href="javascript:;" class="jp-play" tabindex="1">play</a></li>
			  <li><a href="javascript:;" class="jp-pause" tabindex="1">pause</a></li>
			</ul>
		  </div>
	  </div>
      </div>
</div>
</asp:placeholder>
