const Discord = require('discord.js');
const { prefix, token } = require('./config.json');
const axios = require('axios');
const cheerio = require('cheerio');
const request = require('request');
const { title } = require('process');

const client = new Discord.Client();

client.once('ready', () => {
	console.log('Ready! BehemothBot is online.');
});

client.on('message', message => {
	console.log(message.content);
	if (!message.content.startsWith(prefix) || message.author.bot) return;

	const args = message.content.slice(prefix.length).trim().split(/ +/);
	const command = args.shift().toLowerCase();

	if (command === 'ping') {
		message.channel.send('pong')
	}
	else if (command === 'calljs') {
		message.channel.send('BehemothBot.js is active')
	}
	else if (command === 'weather') {
		const country = args[0]
		const state = args[1]
		const city = args[2]

		const getWeather = async () => {
			try {
				const { data } = await axios.get(
					'https://www.wunderground.com/weather/' + country + '/' + state + '/' + city + '/'
				);
				const $ = cheerio.load(data)

				const weather = $('span.test-true.wu-unit.wu-unit-temperature.is-degree-visible.ng-star-inserted > span.wu-value.wu-value-to').text()
				const weatherResults = weather.charAt(2) + weather.charAt(1)
				const Embed = new Discord.MessageEmbed()
					.setColor('#0099ff')
					.setTitle(`Weather for ${city} `)
					.setDescription(`${weatherResults}`)
					.setTimestamp()
					.setFooter('BehemothBot');
				message.channel.send(Embed);
			} catch (error) {
				console.log(error)
				message.channel.send(error)
			}
		}
		getWeather()
	}
	else if (command === 'help') {
		message.channel.send("```ping, calljs, weather, covid, wikipedia, number, reddit, meme, fm```")
		switch (args[0]) {
			case "weather":
				message.channel.send("```Weather: Gets the current temperature for specified city\n-/weather\nCountry: Abbreviated name for specified country\nState/Province: Abbreviated name for specified state or province\nCity: NOT abbreviated name of specified city```");
				break;
			case "covid":
				message.channel.send("```Covid: Gets current covid cases in totals (no arguments)```");
				break;
			case "wikipedia":
				message.channel.send("```Wikipedia: Gets the first part of a random wikipedia article (no arguments)```");
				break;
			case "number":
				message.channel.send("```Number: Calculates the word form of any number\n-/number\nNumber: Specified number```");
				break;
			case "reddit":
				message.channel.send("```Reddit: Gets posts from reddit\nSubreddit: Specified subreddit, will not return anything if sub is banned\nStatus: Control whether to look for new, hot, or top posts```");
				break;
			case "meme":
				message.channel.send("```Meme: Gets a meme from /r/dankmemes, always set to new (no arguments)```");
				break;
			default:
				message.channel.send("Error: Argument not found")
				break;
			case "fm":
				message.channel.send("```Fm: Gets information from last.fm (NOT USING API, THIS INFO IS WEB SCRAPED)\nAccount: Your lastfm username (required)\nFROM HERE ARE ARGUMENTS THAT WORK ALONE\nRecent): gets your most recent song played\nRecentAll: gets your 3 most recent songs\n\nPlays:\nAll: gets total spotify plays\nArtist: Total plays with certain artist, then use the artists name as the next argument. WHEN ARTISTS HAVE SPACES THEY MUST BE SUBSTITUTED WITH '_' OR '+'\n\nArtistCount: Displays total artists played\nSince: gets date you started using lastfm\n\nArtist: specifies artist subcategory\nTop: specifies top subcategory\nSongs: Gets top 5 most played songs by specified artist\nAlbums: Gets top 5 most played albums by specified artist\nArtistSpecified: Final argument is artist specified IF ARTIST HAS MORE THAN ONE NAME SPACES MUST BE SUBSTITUTED WITH EITHER '_' OR '+'```");
        }
    }
	else if (command === 'covid') {
		const getCovid = async () => {
			try {
				const { data } = await axios.get(
					'https://covidusa.net/'
				);
				const $ = cheerio.load(data);

				const covidStats = $('div.stat.py-4 > div.stat-value.display-4.text-warning').text()
				const Embed = new Discord.MessageEmbed()
					.setColor('#0099ff')
					.setTitle('Covid-19 Statistics - US')
					.setDescription(`(1st is current, 2nd is 7 day projection, 3rd is 14 day projection, and 4th is 30 day projection) \n${covidStats}`)
					.setTimestamp()
					.setFooter('BehemothBot');
				message.channel.send(Embed);
			} catch (error) {
				console.log(error)
				message.channel.send(error)
			}
		}
		getCovid()
	}
	else if (command === 'wikipedia') {
		const getCovid = async () => {
			try {
				const { data } = await axios.get(
					'https://en.wikipedia.org/wiki/Special:Random'
				);
				const $ = cheerio.load(data);

				const wikipediatitle = $('h1.firstHeading').text()
				const wikipediacontents = $('p').text()
				const Embed = new Discord.MessageEmbed()
					.setColor('#0099ff')
					.setTitle(wikipediatitle)
					.setDescription(wikipediacontents.substring(0, 500))
					.setTimestamp()
					.setFooter('BehemothBot');
				message.channel.send(Embed);
			} catch (error) {
				console.log(error)
				message.channel.send(error)
			}
		}
		getCovid()
	}
	else if (command === 'number') {
		const number = args[0]
		const getNumber = async () => {
			try {
				const { data } = await axios.get(
					'https://www.calculatorsoup.com/calculators/conversions/numberstowords.php?number=' + number + '&format=words&letter_case=lowercase&action=solve'
				);
				const $ = cheerio.load(data);

				const getAnswer = $('div.still').text()
				const Embed = new Discord.MessageEmbed()
					.setColor('#0099ff')
					.setTitle(number)
					.setDescription(getAnswer)
					.setTimestamp()
					.setFooter('BehemothBot');
				message.channel.send(Embed);
			} catch (error) {
				console.log(error)
				message.channel.send(error)
			}
		}
		getNumber()
	}
	else if (command === 'check') {
		message.channel.send('If you are only seeing this message then BehemothBot is partially down (C# Portion). If you are only seeing this then most commands will not work because BehemothBot is mostly built upon a D# client.')
	}
	else if (command === 'reddit') {
		subreddit = args[0]
		age = args[1]
		const getPost = async () => {
			try {
				const { data } = await axios.get(
					'https://old.reddit.com/r/' + subreddit + '/' + age + '/'
				);
				const $ = cheerio.load(data);

				const a = $("img").attr("href")
				const img = $('img').attr("src")
				const score = $('div.midcol.unvoted > div.score.unvoted').text()
				href = $('div > p.title > a').attr("href");

				var imglink = 'http://old.reddit.com' + href;
				try {
					const { data } = await axios.get(
						imglink
					);

					const $ = cheerio.load(data)
					const imgLarge = $('img.preview').attr("src")

					const Embed = new Discord.MessageEmbed()
						.setColor('#0099ff')
						.setTitle(`Reddit /r/${subreddit}`)
						.setDescription(imglink)
						.setImage(imgLarge)
						.setTimestamp()
						.setFooter(`${score} upvotes - BehemothBot`);
					message.channel.send(Embed);
				} catch (error) {
					console.log(error)
					message.channel.send('This post could not be found, this means the post could be a link from another site' + error)
				}
			} catch (error) {
				console.log(error)
				message.channel.send('This subreddit could not be found, make sure it was typed correctly and that the subreddit exists' + error)
			}
		}
		getPost()
	}
	else if (command === 'meme') {
		const getPost = async () => {
			try {
				const { data } = await axios.get(
					'https://old.reddit.com/r/dankmemes/new/'
				);
				const $ = cheerio.load(data);

				const a = $("img").attr("href")
				const img = $('img').attr("src")
				href = $('div > p.title > a').attr("href");

				var imglink = 'http://old.reddit.com' + href;
				try {
					const { data } = await axios.get(
						imglink
					);

					const $ = cheerio.load(data)
					const imgLarge = $('img.preview').attr("src")

					const Embed = new Discord.MessageEmbed()
						.setColor('#0099ff')
						.setTitle('Meme')
						.setDescription(imglink)
						.setImage(imgLarge)
						.setTimestamp()
						.setFooter('BehemothBot');
					message.channel.send(Embed);
				} catch (error) {
					console.log(error)
				}

			} catch (error) {
				console.log(error);
				message.channel.send(error)
			}
		};
		getPost()
	}
	else if (command === 'ann') {
		const announcement = new Discord.MessageEmbed()
			.setColor('#0099ff')
			.setTitle('ANNOUNCEMENT')
			.setAuthor('DrBehemoth')
			.setDescription('Put whatever you got for christmas in flexing channel')
			.setTimestamp()
			.setFooter('BehemothBot');
		message.channel.send(announcement)
	}
	else if (command === 'fm') {
		const spotifyAccount = args[0]
		switch (args[1]) {
			case "recent":
				const getrecentSong = async () => {
					try {
						const { data } = await axios.get(
							'https://www.last.fm/user/' + spotifyAccount + '/'
						);
						const $ = cheerio.load(data)
						const recentsongName = $('section > table.chartlist tr:nth-child(1) > td.chartlist-name > a').text()
						const recentsongArtist = $('section > table.chartlist tr:nth-child(1) > td.chartlist-artist > a').text()
						const recentsongArtistLink = $('section > table.chartlist tr:nth-child(1) > td.chartlist-artist > a').attr("href")
						const recentsongimgRef = $('section > table.chartlist tr:nth-child(1) > td.chartlist-name > a').attr("href")
						const recentsongartistHref = 'https://www.last.fm/music' + recentsongArtistLink
						const recentsongHref = 'https://www.last.fm' + recentsongimgRef + '/'
						const Embed = new Discord.MessageEmbed()
							.setColor('#0099ff')
							.setTitle(`${spotifyAccount}s Recent Song`)
							.setDescription(`[${recentsongName}](${recentsongHref}) by [${recentsongArtist}](${recentsongartistHref})`)
							.setTimestamp()
							.setFooter('BehemothBot');
						message.channel.send(Embed);

					} catch (error) {
						console.log(error)
						message.channel.send(error)
					}
				}
				getrecentSong()
				break;
			default:
				message.channel.send("Error: Argument not found")
				break;
			case "recentl":
				const getsongLyrics = async () => {
					try {
						const { data } = await axios.get(
							'https://www.last.fm/user/' + spotifyAccount + '/'
						);
						const $ = cheerio.load(data)
						const recentsongName = $('section > table.chartlist tr:nth-child(1) > td.chartlist-name > a').text();
						const { data2 } = await axios.get(
							'https://genius.com/search?q=' + recentsongName
						);
						const $2 = cheerio.load(data2);
						const getSongUrl = $2('div.u-quarter_vertical_margins.u-clickable > a.mini_card').attr("href");
						message.channel.send(getSongUrl);
					} catch (error) {
						console.log(error);
						message.channel.send(error);
					}
				}
				getsongLyrics();
				break;
			case "recentall":
				const getrecentsongAll = async () => {
					try {
						const { data } = await axios.get(
							'https://www.last.fm/user/' + spotifyAccount + '/'
						);
						const $ = cheerio.load(data)
						const recentsongName = $('section > table.chartlist tr:nth-child(1) > td.chartlist-name > a').text()
						const recentsongArtist = $('section > table.chartlist tr:nth-child(1) > td.chartlist-artist > a').text()
						const recentsongArtistLink = $('section > table.chartlist tr:nth-child(1) > td.chartlist-artist > a').attr("href")
						const recentsongimgRef = $('section > table.chartlist tr:nth-child(1) > td.chartlist-name > a').attr("href")
						const recentsongartistHref = 'https://www.last.fm/music' + recentsongArtistLink
						const recentsongHref = 'https://www.last.fm' + recentsongimgRef + '/'

						const recentsongName2 = $('section > table.chartlist tr:nth-child(2) > td.chartlist-name > a').text()
						const recentsongArtist2 = $('section > table.chartlist tr:nth-child(2) > td.chartlist-artist > a').text()
						const recentsongArtistLink2 = $('section > table.chartlist tr:nth-child(2) > td.chartlist-artist > a').attr("href")
						const recentsongimgRef2 = $('section > table.chartlist tr:nth-child(2) > td.chartlist-name > a').attr("href")
						const recentsongartistHref2 = 'https://www.last.fm/music' + recentsongArtistLink2
						const recentsongHref2 = 'https://www.last.fm' + recentsongimgRef2 + '/'

						const recentsongName3 = $('section > table.chartlist tr:nth-child(3) > td.chartlist-name > a').text()
						const recentsongArtist3 = $('section > table.chartlist tr:nth-child(3) > td.chartlist-artist > a').text()
						const recentsongArtistLink3 = $('section > table.chartlist tr:nth-child(3) > td.chartlist-artist > a').attr("href")
						const recentsongimgRef3 = $('section > table.chartlist tr:nth-child(3) > td.chartlist-name > a').attr("href")
						const recentsongartistHref3 = 'https://www.last.fm/music' + recentsongArtistLink3
						const recentsongHref3 = 'https://www.last.fm' + recentsongimgRef3 + '/'
						const Embed = new Discord.MessageEmbed()
							.setColor('#0099ff')
							.setTitle(`${spotifyAccount}s Recent Songs`)
							.setDescription(`[${recentsongName}](${recentsongHref}) by [${recentsongArtist}](${recentsongartistHref}) \n [${recentsongName2}](${recentsongHref2}) by [${recentsongArtist2}](${recentsongartistHref2}) \n [${recentsongName3}](${recentsongHref3}) by [${recentsongArtist3}](${recentsongartistHref3})`)
							.setTimestamp()
							.setFooter('BehemothBot');
						message.channel.send(Embed);

					} catch (error) {
						console.log(error)
						message.channel.send(error)
					}
				}
				getrecentsongAll()
				break;
			case "plays":
				switch (args[2]) {
					case "all":
						const getPlays = async () => {
							try {
								const { data } = await axios.get(
									'https://www.last.fm/user/' + spotifyAccount + '/'
								);
								const $ = cheerio.load(data)
								const scrobbleCount = $('li.header-metadata-item.header-metadata-item--scrobbles > p.header-metadata-display > a').text()
								const Embed = new Discord.MessageEmbed()
									.setColor('#0099ff')
									.setTitle(`Spotify Plays`)
									.setDescription(`${scrobbleCount}`)
									.setTimestamp()
									.setFooter('BehemothBot');
								message.channel.send(Embed);
							} catch (error) {
								console.log(error)
								message.channel.send(error)
							}
						}
						getPlays()
						break;
					default:
						message.channel.send("Error: Argument not found")
						break;
					case "artist":
						const getArtistPlays = async () => {
							const artist = args[3]
		
							try {
								const { data } = await axios.get(
									'https://www.last.fm/user/' + spotifyAccount + '/library/music/' + artist
								);
								const $ = cheerio.load(data)
								const scrobbleartistCountSongs = $('ul.metadata-list li:nth-child(1) > p.metadata-display').text()
								const scrobbleartistCountAlbums = $('ul.metadata-list li:nth-child(2) > p.metadata-display').text()
								const scrobbleartistCountTracks = $('ul.metadata-list li:nth-child(3) > p.metadata-display').text()
								const artistImg = $('span.avatar.library-header-image > img').attr("src")
								const artistName = $('h2.library-header-title').text()
								const Embed = new Discord.MessageEmbed()
									.setColor('#0099ff')
									.setTitle(`${spotifyAccount}s ${artistName} Plays`)
									.setImage(artistImg)
									.setDescription(`${scrobbleartistCountSongs} plays \n ${scrobbleartistCountAlbums} albums \n ${scrobbleartistCountTracks} tracks`)
									.setTimestamp()
									.setFooter('BehemothBot');
								message.channel.send(Embed);
							} catch (error) {
								console.log(error)
								message.channel.send(error)
							}
						}
						getArtistPlays()
						break;
				}
				break;
			case "artistcount":
				const getArtistcount = async () => {
					try {
						const { data } = await axios.get(
							'https://www.last.fm/user/' + spotifyAccount + '/'
						);
						const $ = cheerio.load(data)
						const artistCount = $('li.header-metadata-item.header-metadata-item--artists > p.header-metadata-display > a').text()
						const Embed = new Discord.MessageEmbed()
							.setColor('#099ff')
							.setTitle(`${spotifyAccount}s Artist Count`)
							.setDescription(`${spotifyAccount} has played ${artistCount} artists in total`)
							.setTimestamp()
							.setFooter('BehemothBot')
						message.channel.send(Embed)
					} catch (error) {
						console.log(error)
						message.channel.send(error)
					}
				}
				getArtistcount()
				break;
			case "since":
				const getsinceWhen = async () => {
					try {
						const { data } = await axios.get(
							'https://www.last.fm/user/' + spotifyAccount + '/'
						);
						const $ = cheerio.load(data)
						const sinceWhen = $('span.header-scrobble-since').text()
						const Embed = new Discord.MessageEmbed()
							.setColor('#099ff')
							.setTitle(`${spotifyAccount}`)
							.setDescription(`${spotifyAccount} ${sinceWhen}`)
							.setTimestamp()
							.setFooter('BehemothBot')
						message.channel.send(Embed)
					} catch (error) {
						console.log(error)
						message.channel.send(error)
					}
				}
				getsinceWhen()
				break;
			case "artist":
				switch (args[2]) {
					case "top":
						switch (args[3]) {
							case "songs":
								const gettopsongsBy = async () => {
									try {
										const { data } = await axios.get(
											'https://www.last.fm/user/' + spotifyAccount + '/library/music/' + args[4] + '/+tracks'
										);
										const $ = cheerio.load(data)
										const track1 = $('table.chartlist tr:nth-child(1) > td.chartlist-name > a').text()
										const track2 = $('table.chartlist tr:nth-child(2) > td.chartlist-name > a').text()
										const track3 = $('table.chartlist tr:nth-child(3) > td.chartlist-name > a').text()
										const track4 = $('table.chartlist tr:nth-child(4) > td.chartlist-name > a').text()
										const track5 = $('table.chartlist tr:nth-child(5) > td.chartlist-name > a').text()

										const artistName = $('h2.library-header-title').text()

										const Embed = new Discord.MessageEmbed()
											.setColor('#099ff')
											.setTitle(`${artistName}`)
											.setDescription(`1. ${track1} \n 2. ${track2} \n 3. ${track3} \n 4. ${track4} \n 5. ${track5}`)
											.setTimestamp()
											.setFooter('BehemothBot')
										message.channel.send(Embed)
									} catch (error) {
										console.log(error)
										message.channel.send('An internal error occurred fetching artist top songs')
									}
								}
								gettopsongsBy()
								break;
							case "albums":
								const gettopalbumsBy = async () => {
									try {
										const { data } = await axios.get(
											'https://www.last.fm/user/' + spotifyAccount + '/library/music/' + args[4] + '/+albums'
										);
										const $ = cheerio.load(data)
										const track1 = $('table.chartlist tr:nth-child(1) > td.chartlist-name > a').text()
										const track1plays = $('table.chartlist tr:nth-child(1) > span.chartlist-count-bar-value').text()
										const track2 = $('table.chartlist tr:nth-child(2) > td.chartlist-name > a').text()
										const track3 = $('table.chartlist tr:nth-child(3) > td.chartlist-name > a').text()
										const track4 = $('table.chartlist tr:nth-child(4) > td.chartlist-name > a').text()
										const track5 = $('table.chartlist tr:nth-child(5) > td.chartlist-name > a').text()

										const artistName = $('h2.library-header-title').text()

										const Embed = new Discord.MessageEmbed()
											.setColor('#099ff')
											.setTitle(`${artistName}`)
											.setDescription(`1. ${track1} \n 2. ${track2} \n 3. ${track3} \n 4. ${track4} \n 5. ${track5}`)
											.setTimestamp()
											.setFooter('BehemothBot')
										message.channel.send(Embed)
									} catch (error) {
										console.log(error)
										message.channel.send('An internal error occurred fetching artist top songs')
									}
								}
								gettopalbumsBy()
								break;
							default:
								message.channel.send("Error: Argument not found")
								break;
						}
						break;
					default:
						message.channel.send("Error: Argument not found")
						break;
                }
				break;
		}
	}
})
client.login(token)
