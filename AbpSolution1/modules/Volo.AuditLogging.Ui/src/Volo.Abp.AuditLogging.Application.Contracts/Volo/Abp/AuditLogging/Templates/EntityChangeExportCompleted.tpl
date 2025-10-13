<h3>{{L "EntityChangeExportCompletedSubject"}}</h3>
<p>{{L "ExportReady" model.record_count}}</p>
<p>{{L "DownloadLinkExplanation" model.link_expiration_utc_time}}</p>
<p><a href="{{ model.download_link }}" target="_blank">{{L "DownloadNow"}}</a></p>