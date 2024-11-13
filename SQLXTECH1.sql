set ansi_nulls on
go
set ansi_padding on
go
set quoted_identifier on 
go

create database [XTECTWifi_Database]
go

use [XTECTWifi_Database]
go

create table [dbo].[Wifi_SSID]
(
	[ID_Wifi_SSID] [int] not null identity (1,1) primary key,
	[Name_SSID] [varchar] (50) not null,
	[Wifi_Status] [varchar] (50) not null,
		constraint CK_Wifi_Status_Range check ([Wifi_Status] between 0 and 100)
)
go

select * from [dbo].[Wifi_SSID]

