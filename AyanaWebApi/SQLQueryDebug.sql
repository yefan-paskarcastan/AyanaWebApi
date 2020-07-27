
select *
from RutorListItems
--where HrefNumber = '366503'
order by AddedDate desc, Created desc

select *
from RutorItems
--where id = 538
order by Created desc

select *
from TorrentSoftPosts
where id = 535

select *
from logs
order by created desc;


insert into RutorParseItemInputs(Created, Active, 
			 UriItem, ProxySocks5Addr, 
			 ProxySocks5Port, XPathExprDescription, 
			 XPathExprSpoiler, XPathExprImgs)
values (CURRENT_TIMESTAMP, 1, 
		'http://rutorc6mqdinc4cz.onion/torrent/', '127.0.0.1',
		9050, '//table[@id=''details'']/tr[1]/td[2]',
		'//table[@id=''details'']//tr[1]//td[2]//div[@class=''hidewrap'']', '//table[@id=''details'']//tr[1]//td[2]//img')


insert into DriverRutorTorrentInputs(Created, Active, 
									 TorrentUri, MaxPosterSize, 
									 ProxySocks5Port, ProxySocks5Addr)
values (CURRENT_TIMESTAMP, 1, 
		'http://rutorc6mqdinc4cz.onion/download/', 300,
		9050, '127.0.0.1')