
select *
from RutorListItems
--where HrefNumber = '366503'
order by AddedDate desc, Created desc

select *
from RutorItems
where id = 538

select *
from TorrentSoftPosts
where id = 494

select *
from logs
order by created desc;

select *
from RutorParseItemInputs



update RutorParseItemInputs
set Active = 0
where Id = 2

insert into RutorParseItemInputs(Created, Active, 
			 UriItem, ProxySocks5Addr, 
			 ProxySocks5Port, XPathExprDescription, 
			 XPathExprSpoiler, XPathExprImgs)
values (CURRENT_TIMESTAMP, 1, 
		'http://rutorc6mqdinc4cz.onion/torrent/', '127.0.0.1',
		9050, '//table[@id=''details'']/tr[1]/td[2]',
		'//table[@id=''details'']//tr[1]//td[2]//div[@class=''hidewrap'']', '//table[@id=''details'']//tr[1]//td[2]//img')

select CURRENT_TIMESTAMP

select *
from DriverRutorTorrentInputs

update DriverRutorTorrentInputs
set Active = 0
where Id = 2

insert into DriverRutorTorrentInputs(Created, Active, 
									 TorrentUri, MaxPosterSize, 
									 ProxySocks5Port, ProxySocks5Addr)
values (CURRENT_TIMESTAMP, 1, 
		'http://rutorc6mqdinc4cz.onion/download/', 300,
		9050, '127.0.0.1')

select *
from ImghostParsingInputs