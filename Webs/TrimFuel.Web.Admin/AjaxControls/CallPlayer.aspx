<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CallPlayer.aspx.cs" Inherits="TrimFuel.Web.Admin.AjaxControls.CallPlayer" %>

<script type="text/javascript">
    $(document).ready(function () {
        createAudioPlayer(
			$('#call-container-<%# Request["callID"] %>'),
			300,
			70,
			'/dotNet/AjaxControls/CallListen.aspx?ContactID=<%# Request["contactID"] %>',
            '<%# Request["callID"] %>'
		)
    })
    function createAudioPlayer(el, width, height, url, callID) {
        $(el).append(
            "<object id='mPlayer-" + callID + "' width='" + width + "' height='" + height + "' classid='CLSID:22d6f312-b0f6-11d0-94ab-0080c74c7e95'" +
            "codebase='http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=5,1,52,701' standby='Loading Microsoft Windows Media Player components...' type='application/x-oleobject'>" +
            "<param name='fileName' value='" + url + "'><param name='animationatStart' value='true'><param name='transparentatStart' value='true'><param name='autoStart' value='false'> <param name='showControls' value='true'><param name='ShowAudioControls' value='true'><param name='ShowStatusBar' value='true'><param name='loop' value='false'>" +
            "<embed type='application/x-mplayer2' pluginspage='http://microsoft.com/windows/mediaplayer/en/download/' name='mediaPlayer' displaysize='4' autosize='-1' bgcolor='darkblue'  showcontrols='1' showtracker='1' showdisplay='0' showstatusbar='1' videoborder3d='-1'" +
            " width='" + width + "' height='" + height + "' src='" + url + "' autostart='0' designtimesp='5311' loop='0'></object>");
        return $(el);
    }
</script>

<div id='call-container-<%# Request["callID"] %>'>
</div>
