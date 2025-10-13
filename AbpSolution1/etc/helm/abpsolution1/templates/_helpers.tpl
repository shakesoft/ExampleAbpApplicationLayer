,
{{- define "abpsolution1.hosts.webpublic" -}}
{{- print "https://" (.Values.global.hosts.webpublic | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
{{- define "abpsolution1.hosts.httpapi" -}}
{{- print "https://" (.Values.global.hosts.httpapi | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
{{- define "abpsolution1.hosts.angular" -}}
{{- print "https://" (.Values.global.hosts.angular | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
{{- define "abpsolution1.hosts.authserver" -}}
{{- print "https://" (.Values.global.hosts.authserver | replace "[RELEASE_NAME]" .Release.Name) -}}
{{- end -}}
