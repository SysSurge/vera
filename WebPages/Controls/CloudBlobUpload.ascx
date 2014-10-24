<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CloudBlobUpload.ascx.cs" Inherits="VeraWAF.WebPages.Controls.CloudBlobUpload" %>
<div style="border: 1px solid #000">

    <div id="ChooseFiles" type="button" style="clear:both;font-size:large">Upload files to the Azure Blob Storage</div>
    <label for="FileList">File list:</label>
    <div id="FileList" style="clear:both;overflow:visible"></div>
    <div id="TotalProgress" style="clear:both"></div>
    <div style="clear:both;height:1em"></div>

    <div id="silverlightControlHost">
        <object id="SlPlugin" data="data:application/x-silverlight-2," type="application/x-silverlight-2" width="200" height="200" style="border: 1px solid green">
		    <param name="source" value="/ClientBin/AzureSilverlightFileUploaderPlugIn.xap"/>
		    <param name="onError" value="onSilverlightError" />
		    <param name="background" value="white" />
		    <param name="minRuntimeVersion" value="5.0.61118.0" />
		    <param name="autoUpgrade" value="true" />
            <param name="initParams" value="MaxFileSizeKB=1000000000,MaxUploads=10,AllowMultipleFileUpload=true,CollectionChanged_event=CollectionChanged,TotalPercentageChanged_event=TotalPercentageChangedEvent,AllFilesFinished_event=AllFilesFinishedEvent,FileListEmpty_event=FileListEmptyEvent" />
		    <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=4.0.50826.0" style="text-decoration:none">
 			    <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight" style="border-style:none"/>
		    </a>
	    </object><iframe id="_sl_historyFrame" style="visibility:hidden;height:0px;width:0px;border:0px"></iframe>
    </div>
    <div style="clear:both;height:1em"></div>
    <button id="UploadFiles" type="button" class="uploadButton" runat="server">Upload</button>
    <button id="ClearFileList" type="button">Clear</button>

    <div id="UploadFinished"></div>
</div>


<asp:FileUpload ID="FileUploadControl" runat="server" />
