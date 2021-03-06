# Common logic for NuGet installation scripts

[System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms") 

function ReportError
{
Param(
  [string]$message
 )
	throw $message
}

function ReportInfo([string] $message, [string] $caption)
{
	Write-Host $message -background yellow

	[System.Windows.Forms.MessageBox]::Show($message, $caption)
}

function ReportApplicationInsightsConfigNotFound()
{
	ReportInfo "Add Application Insights to your project to send log data to the Application Insights portal." "Can’t find ApplicationInsights.config"
}

function GetOrCreateElement
{
	Param(
 		[System.Xml.Linq.XContainer] $container,
 		[System.Xml.Linq.XName] $name)
        
	$element = $container.Element($name)
	if (!$element)
	{
		$element = New-Object -TypeName System.Xml.Linq.XElement -ArgumentList $name
    	$container.Add($element)
	}

	return $element
}

function ValidateProject
{
	Param([Object]$project)
	
	if(!$project)
	{
		ReportError "The Application Insights logging adapter package can’t determine the project you want to apply it to."
	}
}

function GetAIConfigPath
{
	Param([Object] $project)
	
	$aiConfigProjectItem = $project.ProjectItems | where {$_.Name -eq "ApplicationInsights.config"}

	$aiConfigPath = $aiConfigProjectItem.Properties | where {$_.Name -eq "LocalPath"}
	
	if(!$aiConfigPath)
	{
		throw "Can’t find ApplicationInsights.config. Add Application Insights to your project and retry."
	}

	return $aiConfigPath.Value
}

function GetWebConfigPath
{
	Param([object] $project)
	
	$webConfigProjectItem = $project.ProjectItems | where {$_.Name -eq "Web.config"}

	$webConfigPath = $webConfigProjectItem.Properties | where {$_.Name -eq "LocalPath"}

	if(!$webConfigPath)
	{
		throw "Can't find Web.config. Make sure you are targetting an appropriate project type."
	}
	
	return $webConfigPath.Value
}

function LoadXml
{
	Param([string] $filePath)
	
	$fileStream = New-Object -TypeName System.IO.FileStream -ArgumentList $filePath, "Open", "Read"
	
	try
	{
		$xml = [System.Xml.Linq.XElement]::Load($fileStream, "None");	#PreserveWhitespace
	}
	catch
	{
		ReportError "Couldn't load XML from " + $filePath + " " + $_
		return
	}
	finally
	{
		if($fileStream)
		{
			$fileStream.Dispose()
		}
	}
	
	return $xml
}

function GetInstrumentationKey
{
	Param([Object] $aiConfigPath)
	
	$xml = LoadXml $aiConfigPath
	
	$xmlns = [System.Xml.Linq.XNamespace]::Get("http://schemas.microsoft.com/ApplicationInsights/2013/Settings")
	$instrumentationKeyElement = $xml.Element($xmlns + "InstrumentationKey")

	if(!$instrumentationKeyElement)
	{
		ReportError "Can’t find the InstrumentationKey element in ApplicationInsights.config. Create an Application Insights resource in Microsoft Azure, then get a key from the Quick Start tile."
		return
	}

	return $instrumentationKeyElement.Value
}

function DoesAIConfigExist([Object] $project)
{
	$aiConfigProjectItem = $project.ProjectItems | where {$_.Name -eq "ApplicationInsights.config"}

	if(!$aiConfigProjectItem)
	{
		return $false
	}

	return $true
}

function CreateAttribute
{
	Param([String] $name, [Object] $value)
	
	return New-Object -TypeName System.Xml.Linq.XAttribute -ArgumentList $name, $value
}

function LoadWebConfig
{
	Param([Object] $project)
	
	$webConfigPath = GetWebConfigPath $project
	
	return LoadXml $webConfigPath
}

function SaveWebConfig
{
	Param([Object] $project, [Object] $xml)
	
	$filePath = GetWebConfigPath $project
	
	try
	{
		$xml.Save($filePath, "None");	#"DisableFormatting"
	}
	catch
	{
		ReportError "Couldn't write changes to web.config. Make sure that web.config is not open in Visual Studio and try again. " + $_
		return
	}
}

function RemoveIfNoChildren([System.Xml.Linq.XElement] $element)
{
	if([bool]$element.HasElements -eq $false)
	{
		$element.Remove()
	}
}
# SIG # Begin signature block
# MIIaqQYJKoZIhvcNAQcCoIIamjCCGpYCAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQUorE+1qtaMPZyEcF9l07yA46+
# UT6gghWCMIIEwzCCA6ugAwIBAgITMwAAAIz/8uUYHhYhIgAAAAAAjDANBgkqhkiG
# 9w0BAQUFADB3MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4G
# A1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSEw
# HwYDVQQDExhNaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EwHhcNMTUxMDA3MTgxNDAz
# WhcNMTcwMTA3MTgxNDAzWjCBszELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hp
# bmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jw
# b3JhdGlvbjENMAsGA1UECxMETU9QUjEnMCUGA1UECxMebkNpcGhlciBEU0UgRVNO
# OjU4NDctRjc2MS00RjcwMSUwIwYDVQQDExxNaWNyb3NvZnQgVGltZS1TdGFtcCBT
# ZXJ2aWNlMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA4W+LEnfuZm/G
# IvSqVPm++Ck9A/SF27VL7uz2UVwcplyRlFzPcVu5oLD4/hnjqwR28E3X7Fz1SHwD
# XpaRyCFCi3rXEZDJIYq3AxZYINPoc9D75eLpbjxdjslrZjOEZKT3YCzZB/gHX/v6
# ubvwP+oiDSsYV0t/GuWLkMtT49ngakuI6j0bamkAD/WOPB9aBa+KekFwpMn7H+/j
# LP2S7y1fiGErxBwI1qmbBR/g7N4Aka4LOzkxOKVFWNdOWAhvChKomkpiWPyhb9bY
# 4+CqcpYvCHyq1V8siMzd0bUZYzibnYL5aHoMWKVgxZRqZKTvRcr5s1NQtHkucERK
# 4CkAb4MhqQIDAQABo4IBCTCCAQUwHQYDVR0OBBYEFOZJqXDBCcJz5PLcr2XHyiAb
# YqdkMB8GA1UdIwQYMBaAFCM0+NlSRnAK7UD7dvuzK7DDNbMPMFQGA1UdHwRNMEsw
# SaBHoEWGQ2h0dHA6Ly9jcmwubWljcm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1Y3Rz
# L01pY3Jvc29mdFRpbWVTdGFtcFBDQS5jcmwwWAYIKwYBBQUHAQEETDBKMEgGCCsG
# AQUFBzAChjxodHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpL2NlcnRzL01pY3Jv
# c29mdFRpbWVTdGFtcFBDQS5jcnQwEwYDVR0lBAwwCgYIKwYBBQUHAwgwDQYJKoZI
# hvcNAQEFBQADggEBAIsRhQk0uISBb0rdX57b2fsvYaNCa9h9SUn6vl26eMAiWEoI
# wDOTALzioSHJPwLKx3CV+pBnDy8MTIKEjacHJhMJ/m8b5PFDopM53NbkVE3NgqjF
# id4O1YH5mFjJDCi0M2udQL9sYsIn5wC6+mxlz15jnc72kCc34cU+1HgOU6UPGURM
# XZzE67qms2NgE+FIPMNbHw7PfI8PSHZz/W9Y+oyCsyJlggc4lMCK97AKo6weBMNH
# Zh8KqwLxb6CDM/UuYAs0UvflmvpbITPlCssYJtdzM+hF6NdMvIkUw0BGtqsIZUZK
# q2sOk0RYOYL4BYDWTBPhPWpKpDKFYUKpgrkP94kwggTsMIID1KADAgECAhMzAAAB
# Cix5rtd5e6asAAEAAAEKMA0GCSqGSIb3DQEBBQUAMHkxCzAJBgNVBAYTAlVTMRMw
# EQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVN
# aWNyb3NvZnQgQ29ycG9yYXRpb24xIzAhBgNVBAMTGk1pY3Jvc29mdCBDb2RlIFNp
# Z25pbmcgUENBMB4XDTE1MDYwNDE3NDI0NVoXDTE2MDkwNDE3NDI0NVowgYMxCzAJ
# BgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25k
# MR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xDTALBgNVBAsTBE1PUFIx
# HjAcBgNVBAMTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjCCASIwDQYJKoZIhvcNAQEB
# BQADggEPADCCAQoCggEBAJL8bza74QO5KNZG0aJhuqVG+2MWPi75R9LH7O3HmbEm
# UXW92swPBhQRpGwZnsBfTVSJ5E1Q2I3NoWGldxOaHKftDXT3p1Z56Cj3U9KxemPg
# 9ZSXt+zZR/hsPfMliLO8CsUEp458hUh2HGFGqhnEemKLwcI1qvtYb8VjC5NJMIEb
# e99/fE+0R21feByvtveWE1LvudFNOeVz3khOPBSqlw05zItR4VzRO/COZ+owYKlN
# Wp1DvdsjusAP10sQnZxN8FGihKrknKc91qPvChhIqPqxTqWYDku/8BTzAMiwSNZb
# /jjXiREtBbpDAk8iAJYlrX01boRoqyAYOCj+HKIQsaUCAwEAAaOCAWAwggFcMBMG
# A1UdJQQMMAoGCCsGAQUFBwMDMB0GA1UdDgQWBBSJ/gox6ibN5m3HkZG5lIyiGGE3
# NDBRBgNVHREESjBIpEYwRDENMAsGA1UECxMETU9QUjEzMDEGA1UEBRMqMzE1OTUr
# MDQwNzkzNTAtMTZmYS00YzYwLWI2YmYtOWQyYjFjZDA1OTg0MB8GA1UdIwQYMBaA
# FMsR6MrStBZYAck3LjMWFrlMmgofMFYGA1UdHwRPME0wS6BJoEeGRWh0dHA6Ly9j
# cmwubWljcm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1Y3RzL01pY0NvZFNpZ1BDQV8w
# OC0zMS0yMDEwLmNybDBaBggrBgEFBQcBAQROMEwwSgYIKwYBBQUHMAKGPmh0dHA6
# Ly93d3cubWljcm9zb2Z0LmNvbS9wa2kvY2VydHMvTWljQ29kU2lnUENBXzA4LTMx
# LTIwMTAuY3J0MA0GCSqGSIb3DQEBBQUAA4IBAQCmqFOR3zsB/mFdBlrrZvAM2PfZ
# hNMAUQ4Q0aTRFyjnjDM4K9hDxgOLdeszkvSp4mf9AtulHU5DRV0bSePgTxbwfo/w
# iBHKgq2k+6apX/WXYMh7xL98m2ntH4LB8c2OeEti9dcNHNdTEtaWUu81vRmOoECT
# oQqlLRacwkZ0COvb9NilSTZUEhFVA7N7FvtH/vto/MBFXOI/Enkzou+Cxd5AGQfu
# FcUKm1kFQanQl56BngNb/ErjGi4FrFBHL4z6edgeIPgF+ylrGBT6cgS3C6eaZOwR
# XU9FSY0pGi370LYJU180lOAWxLnqczXoV+/h6xbDGMcGszvPYYTitkSJlKOGMIIF
# vDCCA6SgAwIBAgIKYTMmGgAAAAAAMTANBgkqhkiG9w0BAQUFADBfMRMwEQYKCZIm
# iZPyLGQBGRYDY29tMRkwFwYKCZImiZPyLGQBGRYJbWljcm9zb2Z0MS0wKwYDVQQD
# EyRNaWNyb3NvZnQgUm9vdCBDZXJ0aWZpY2F0ZSBBdXRob3JpdHkwHhcNMTAwODMx
# MjIxOTMyWhcNMjAwODMxMjIyOTMyWjB5MQswCQYDVQQGEwJVUzETMBEGA1UECBMK
# V2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0
# IENvcnBvcmF0aW9uMSMwIQYDVQQDExpNaWNyb3NvZnQgQ29kZSBTaWduaW5nIFBD
# QTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBALJyWVwZMGS/HZpgICBC
# mXZTbD4b1m/My/Hqa/6XFhDg3zp0gxq3L6Ay7P/ewkJOI9VyANs1VwqJyq4gSfTw
# aKxNS42lvXlLcZtHB9r9Jd+ddYjPqnNEf9eB2/O98jakyVxF3K+tPeAoaJcap6Vy
# c1bxF5Tk/TWUcqDWdl8ed0WDhTgW0HNbBbpnUo2lsmkv2hkL/pJ0KeJ2L1TdFDBZ
# +NKNYv3LyV9GMVC5JxPkQDDPcikQKCLHN049oDI9kM2hOAaFXE5WgigqBTK3S9dP
# Y+fSLWLxRT3nrAgA9kahntFbjCZT6HqqSvJGzzc8OJ60d1ylF56NyxGPVjzBrAlf
# A9MCAwEAAaOCAV4wggFaMA8GA1UdEwEB/wQFMAMBAf8wHQYDVR0OBBYEFMsR6MrS
# tBZYAck3LjMWFrlMmgofMAsGA1UdDwQEAwIBhjASBgkrBgEEAYI3FQEEBQIDAQAB
# MCMGCSsGAQQBgjcVAgQWBBT90TFO0yaKleGYYDuoMW+mPLzYLTAZBgkrBgEEAYI3
# FAIEDB4KAFMAdQBiAEMAQTAfBgNVHSMEGDAWgBQOrIJgQFYnl+UlE/wq4QpTlVnk
# pDBQBgNVHR8ESTBHMEWgQ6BBhj9odHRwOi8vY3JsLm1pY3Jvc29mdC5jb20vcGtp
# L2NybC9wcm9kdWN0cy9taWNyb3NvZnRyb290Y2VydC5jcmwwVAYIKwYBBQUHAQEE
# SDBGMEQGCCsGAQUFBzAChjhodHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20vcGtpL2Nl
# cnRzL01pY3Jvc29mdFJvb3RDZXJ0LmNydDANBgkqhkiG9w0BAQUFAAOCAgEAWTk+
# fyZGr+tvQLEytWrrDi9uqEn361917Uw7LddDrQv+y+ktMaMjzHxQmIAhXaw9L0y6
# oqhWnONwu7i0+Hm1SXL3PupBf8rhDBdpy6WcIC36C1DEVs0t40rSvHDnqA2iA6VW
# 4LiKS1fylUKc8fPv7uOGHzQ8uFaa8FMjhSqkghyT4pQHHfLiTviMocroE6WRTsgb
# 0o9ylSpxbZsa+BzwU9ZnzCL/XB3Nooy9J7J5Y1ZEolHN+emjWFbdmwJFRC9f9Nqu
# 1IIybvyklRPk62nnqaIsvsgrEA5ljpnb9aL6EiYJZTiU8XofSrvR4Vbo0HiWGFzJ
# NRZf3ZMdSY4tvq00RBzuEBUaAF3dNVshzpjHCe6FDoxPbQ4TTj18KUicctHzbMrB
# 7HCjV5JXfZSNoBtIA1r3z6NnCnSlNu0tLxfI5nI3EvRvsTxngvlSso0zFmUeDord
# EN5k9G/ORtTTF+l5xAS00/ss3x+KnqwK+xMnQK3k+eGpf0a7B2BHZWBATrBC7E7t
# s3Z52Ao0CW0cgDEf4g5U3eWh++VHEK1kmP9QFi58vwUheuKVQSdpw5OPlcmN2Jsh
# rg1cnPCiroZogwxqLbt2awAdlq3yFnv2FoMkuYjPaqhHMS+a3ONxPdcAfmJH0c6I
# ybgY+g5yjcGjPa8CQGr/aZuW4hCoELQ3UAjWwz0wggYHMIID76ADAgECAgphFmg0
# AAAAAAAcMA0GCSqGSIb3DQEBBQUAMF8xEzARBgoJkiaJk/IsZAEZFgNjb20xGTAX
# BgoJkiaJk/IsZAEZFgltaWNyb3NvZnQxLTArBgNVBAMTJE1pY3Jvc29mdCBSb290
# IENlcnRpZmljYXRlIEF1dGhvcml0eTAeFw0wNzA0MDMxMjUzMDlaFw0yMTA0MDMx
# MzAzMDlaMHcxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYD
# VQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xITAf
# BgNVBAMTGE1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQTCCASIwDQYJKoZIhvcNAQEB
# BQADggEPADCCAQoCggEBAJ+hbLHf20iSKnxrLhnhveLjxZlRI1Ctzt0YTiQP7tGn
# 0UytdDAgEesH1VSVFUmUG0KSrphcMCbaAGvoe73siQcP9w4EmPCJzB/LMySHnfL0
# Zxws/HvniB3q506jocEjU8qN+kXPCdBer9CwQgSi+aZsk2fXKNxGU7CG0OUoRi4n
# rIZPVVIM5AMs+2qQkDBuh/NZMJ36ftaXs+ghl3740hPzCLdTbVK0RZCfSABKR2YR
# JylmqJfk0waBSqL5hKcRRxQJgp+E7VV4/gGaHVAIhQAQMEbtt94jRrvELVSfrx54
# QTF3zJvfO4OToWECtR0Nsfz3m7IBziJLVP/5BcPCIAsCAwEAAaOCAaswggGnMA8G
# A1UdEwEB/wQFMAMBAf8wHQYDVR0OBBYEFCM0+NlSRnAK7UD7dvuzK7DDNbMPMAsG
# A1UdDwQEAwIBhjAQBgkrBgEEAYI3FQEEAwIBADCBmAYDVR0jBIGQMIGNgBQOrIJg
# QFYnl+UlE/wq4QpTlVnkpKFjpGEwXzETMBEGCgmSJomT8ixkARkWA2NvbTEZMBcG
# CgmSJomT8ixkARkWCW1pY3Jvc29mdDEtMCsGA1UEAxMkTWljcm9zb2Z0IFJvb3Qg
# Q2VydGlmaWNhdGUgQXV0aG9yaXR5ghB5rRahSqClrUxzWPQHEy5lMFAGA1UdHwRJ
# MEcwRaBDoEGGP2h0dHA6Ly9jcmwubWljcm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1
# Y3RzL21pY3Jvc29mdHJvb3RjZXJ0LmNybDBUBggrBgEFBQcBAQRIMEYwRAYIKwYB
# BQUHMAKGOGh0dHA6Ly93d3cubWljcm9zb2Z0LmNvbS9wa2kvY2VydHMvTWljcm9z
# b2Z0Um9vdENlcnQuY3J0MBMGA1UdJQQMMAoGCCsGAQUFBwMIMA0GCSqGSIb3DQEB
# BQUAA4ICAQAQl4rDXANENt3ptK132855UU0BsS50cVttDBOrzr57j7gu1BKijG1i
# uFcCy04gE1CZ3XpA4le7r1iaHOEdAYasu3jyi9DsOwHu4r6PCgXIjUji8FMV3U+r
# kuTnjWrVgMHmlPIGL4UD6ZEqJCJw+/b85HiZLg33B+JwvBhOnY5rCnKVuKE5nGct
# xVEO6mJcPxaYiyA/4gcaMvnMMUp2MT0rcgvI6nA9/4UKE9/CCmGO8Ne4F+tOi3/F
# NSteo7/rvH0LQnvUU3Ih7jDKu3hlXFsBFwoUDtLaFJj1PLlmWLMtL+f5hYbMUVbo
# nXCUbKw5TNT2eb+qGHpiKe+imyk0BncaYsk9Hm0fgvALxyy7z0Oz5fnsfbXjpKh0
# NbhOxXEjEiZ2CzxSjHFaRkMUvLOzsE1nyJ9C/4B5IYCeFTBm6EISXhrIniIh0EPp
# K+m79EjMLNTYMoBMJipIJF9a6lbvpt6Znco6b72BJ3QGEe52Ib+bgsEnVLaxaj2J
# oXZhtG6hE6a/qkfwEm/9ijJssv7fUciMI8lmvZ0dhxJkAj0tr1mPuOQh5bWwymO0
# eFQF1EEuUKyUsKV4q7OglnUa2ZKHE3UiLzKoCG6gW4wlv6DvhMoh1useT8ma7kng
# 9wFlb4kLfchpyOZu6qeXzjEp/w7FW1zYTRuh2Povnj8uVRZryROj/TGCBJEwggSN
# AgEBMIGQMHkxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYD
# VQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xIzAh
# BgNVBAMTGk1pY3Jvc29mdCBDb2RlIFNpZ25pbmcgUENBAhMzAAABCix5rtd5e6as
# AAEAAAEKMAkGBSsOAwIaBQCggaowGQYJKoZIhvcNAQkDMQwGCisGAQQBgjcCAQQw
# HAYKKwYBBAGCNwIBCzEOMAwGCisGAQQBgjcCARUwIwYJKoZIhvcNAQkEMRYEFK46
# t0ghlwztG1wxCDEKCSgB/DjQMEoGCisGAQQBgjcCAQwxPDA6oCCAHgBOAHUARwBl
# AHQAQwBvAG0AbQBvAG4ALgBwAHMAMaEWgBRodHRwOi8vbWljcm9zb2Z0LmNvbTAN
# BgkqhkiG9w0BAQEFAASCAQA767egh0e33vt8m8MozKvH3Ce3oNRE+JrvMUhIkaxH
# r7lt3+eLzfkDzdlaMiQUedR9P7ujVhRxuRx+cIO1UEBwSERFbE7xlpQhH5UKeP/o
# lwfNUKdPYq2qEU4yDaw+2OhUuwHY7Lp1ZHnK3YA8Di4ADDxEKZ4hXSd2Y0pY9ACM
# EyunxxPmD9WKwLX05VE7Q7677wVW+8eeXtXJ3WvYwE0o1FreMThHAjEdvW8hSNc8
# hjGx02g2XniozyD14vNgo7s1gavOb3CezxF9IQfK5DYPeyQ6CBTzbodwCZUBRSuW
# kk6zlywwnz1fri8RVJOhUu3TIoehK1bSYDouGJ2hT1gjoYICKDCCAiQGCSqGSIb3
# DQEJBjGCAhUwggIRAgEBMIGOMHcxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNo
# aW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29y
# cG9yYXRpb24xITAfBgNVBAMTGE1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQQITMwAA
# AIz/8uUYHhYhIgAAAAAAjDAJBgUrDgMCGgUAoF0wGAYJKoZIhvcNAQkDMQsGCSqG
# SIb3DQEHATAcBgkqhkiG9w0BCQUxDxcNMTUxMjIyMDU1NzM0WjAjBgkqhkiG9w0B
# CQQxFgQUDetiWkq4xMSsS3KvfUUhy+84VYUwDQYJKoZIhvcNAQEFBQAEggEAWcjb
# 2dVXpOKn6U0Km5GiQRR5KdsV2sLrFrBNMyP9q6HfJtBk1HI9zy1oByYYDnpfCygd
# 2PLRqIBO/yR36NqUQpRLlfZZ3DVOA5dzvmT7XM0N3MezLMO8njylj1dfhiP11O5S
# PLxXRIjTxUpoCm3kDjjqSwIJaBihx+aAOy8apbL0h4sJQSrp80JUOdEnL40zGNAF
# o28OSS16coEXFOR+kP93pCcKuA2sPQGMBK5wEIEXVFPuXV6PbVvG7056uzkpWoI0
# 72T7M3A55PJ8JV26ApAHI+LlIdtyIzcdgXvX0ENcbV3nu7oP5ptVM0Th/EBclqLZ
# LRd2hAk+sv5RAvpWeQ==
# SIG # End signature block
