/world
	sleep_offline = FALSE
	loop_checks = FALSE

/world/proc/RunTest()
	log << "Initial value of sleep_offline: [sleep_offline]"
	sleep_offline = FALSE

	// Intentionally slow down startup for testing purposes
	for(var/i in 1 to 10000000)
		dab()
	TgsNew(new /datum/tgs_event_handler/impl, TGS_SECURITY_SAFE)

	var/list/channels = TgsChatChannelInfo()
	if(!length(channels))
		FailTest("Expected some chat channels!")

	StartAsync()

/proc/dab()
	return

/proc/StartAsync()
	set waitfor = FALSE
	Run()

/proc/Run()
	sleep(60)
	world.TgsChatBroadcast(new /datum/tgs_message_content("World Initialized"))
	var/datum/tgs_message_content/response = new("Embed support test1")
	response.embed = new()
	response.embed.description = "desc"
	response.embed.title = "title"
	response.embed.colour = "#00FF00"
	response.embed.author = new /datum/tgs_chat_embed/provider/author("Dominion")
	response.embed.author.url = "https://github.com/Cyberboss"
	response.embed.timestamp = time2text(world.timeofday, "YYYY-MM-DD hh:mm:ss")
	response.embed.url = "https://github.com/tgstation/tgstation-server"
	response.embed.fields = list()
	var/datum/tgs_chat_embed/field/field = new("field1","value1")
	field.is_inline = TRUE
	response.embed.fields += field
	field = new("field2","value2")
	field.is_inline = TRUE
	response.embed.fields += field
	field = new("field3","value3")
	response.embed.fields += field
	response.embed.footer = new /datum/tgs_chat_embed/footer("Footer text")
	world.TgsChatBroadcast(response)
	world.TgsInitializationComplete()

	startup_complete = TRUE
	if(run_bridge_test)
		CheckBridgeLimits()

/world/Topic(T, Addr, Master, Keys)
	if(findtext(T, "tgs_integration_test_tactics3") == 0)
		log << "Topic: [T]"
	else
		log << "tgs_integration_test_tactics3 <TOPIC SUPPRESSED>"
	. =  HandleTopic(T)
	log << "Response: [.]"

var/startup_complete
var/run_bridge_test

/world/proc/HandleTopic(T)
	TGS_TOPIC

	var/list/data = params2list(T)
	var/special_tactics = data["tgs_integration_test_special_tactics"]
	if(special_tactics)
		RebootAsync()
		return "ack"

	var/tactics2 = data["tgs_integration_test_tactics2"]
	if(tactics2)
		if(startup_complete)
			CheckBridgeLimits()
		else
			run_bridge_test = TRUE
		return "ack2"

	// Topic limit tests
	// Receive
	var/tactics3 = data["tgs_integration_test_tactics3"]
	if(tactics3)
		var/list/json = json_decode(tactics3)
		if(!json || !istext(json["payload"]) || !istext(json["size"]))
			return "fail"

		var/size = text2num(json["size"])
		var/payload = json["payload"]
		if(length(payload) != size)
			return "fail"

		return "pass"

	// Send
	var/tactics4 = data["tgs_integration_test_tactics4"]
	if(tactics4)
		var/size = isnum(tactics4) ? tactics4 : text2num(tactics4)
		if(!isnum(size))
			FailTest("tgs_integration_test_tactics4 wasn't a number!")

		var/payload = create_payload(size)
		return payload

	// Chat overload
	var/tactics5 = data["tgs_integration_test_tactics5"]
	if(tactics5)
		TgsChatBroadcast(new /datum/tgs_message_content(create_payload(3000)))
		return "sent"

	// Bridge response queuing
	var/tactics6 = data["tgs_integration_test_tactics6"]
	if(tactics6)
		// hack hack, calling world.TgsChatChannelInfo() will try to delay until the channels come back
		var/datum/tgs_api/v5/api = TGS_READ_GLOBAL(tgs)
		if (length(api.chat_channels))
			return "channels_present!"

		DetachedChatMessageQueuing()
		return "queued"

	var/tactics7 = data["tgs_integration_test_tactics7"]
	if(tactics7)
		var/list/channels = TgsChatChannelInfo()
		return "[length(channels)]"

	var/tactics8 = data["tgs_integration_test_tactics8"]
	if(tactics8)
		return received_health_check ? "received health check" : "did not receive health check"

	TgsChatBroadcast(new /datum/tgs_message_content("Recieved non-tgs topic: `[T]`"))

	return "feck"

// Look I always forget how waitfor = FALSE works
/proc/DetachedChatMessageQueuing()
	set waitfor = FALSE
	DetachedChatMessageQueuingP2()

/proc/DetachedChatMessageQueuingP2()
	sleep(1)
	DetachedChatMessageQueuingP3()

/proc/DetachedChatMessageQueuingP3()
	set waitfor = FALSE
	world.TgsChatBroadcast(new /datum/tgs_message_content("1/3 queued detached chat messages"))
	world.TgsChatBroadcast(new /datum/tgs_message_content("2/3 queued detached chat messages"))
	world.TgsChatBroadcast(new /datum/tgs_message_content("3/3 queued detached chat messages"))

/world/Reboot(reason)
	TgsChatBroadcast("World Rebooting")
	TgsReboot()

var/received_health_check = FALSE

/datum/tgs_event_handler/impl
	receive_health_checks = TRUE

/datum/tgs_event_handler/impl/HandleEvent(event_code, ...)
	set waitfor = FALSE

	if(event_code == TGS_EVENT_HEALTH_CHECK)
		received_health_check = TRUE
	else if(event_code == TGS_EVENT_WATCHDOG_DETACH)
		// hack hack, calling world.TgsChatChannelInfo() will try to delay until the channels come back
		var/datum/tgs_api/v5/api = TGS_READ_GLOBAL(tgs)
		if(length(api.chat_channels))
			FailTest("Expected no chat channels after detach!")

	world.TgsChatBroadcast(new /datum/tgs_message_content("Recieved event: `[json_encode(args)]`"))

/world/Export(url)
	if(length(url) < 1000)
		log << "Export: [url]"
	return ..()

/proc/RebootAsync()
	set waitfor = FALSE
	world.TgsChatBroadcast(new /datum/tgs_message_content("Rebooting after 3 seconds"));
	world.log << "About to sleep. sleep_offline: [world.sleep_offline]"
	sleep(30)
	world.log << "Done sleep, calling Reboot"
	world.Reboot()

/datum/tgs_chat_command/embeds_test
	name = "embeds_test"
	help_text = "dumps an embed"

/datum/tgs_chat_command/embeds_test/Run(datum/tgs_chat_user/sender, params)
	var/datum/tgs_message_content/response = new("Embed support test2")
	response.embed = new()
	response.embed.description = "desc"
	response.embed.title = "title"
	response.embed.colour = "#0000FF"
	response.embed.author = new /datum/tgs_chat_embed/provider/author("Dominion")
	response.embed.author.url = "https://github.com/Cyberboss"
	response.embed.timestamp = time2text(world.timeofday, "YYYY-MM-DD hh:mm:ss")
	response.embed.url = "https://github.com/tgstation/tgstation-server"
	response.embed.fields = list()
	var/datum/tgs_chat_embed/field/field = new("field1","value1")
	response.embed.fields += field
	field = new("field2","value2")
	field.is_inline = TRUE
	response.embed.fields += field
	field = new("field3","value3")
	field.is_inline = TRUE
	response.embed.fields += field
	response.embed.footer = new /datum/tgs_chat_embed/footer("Footer text")
	return response

/datum/tgs_chat_command/response_overload_test
	name = "response_overload_test"
	help_text = "returns a massive string that probably won't display in a chat client but is used to test topic response chunking"

/datum/tgs_chat_command/response_overload_test/Run(datum/tgs_chat_user/sender, params)
	// DMAPI5_TOPIC_RESPONSE_LIMIT
	var/limit = 65528
	// this actually gets doubled because it's in two fields for backwards compatibility, but that's fine
	var/datum/tgs_message_content/response = new(create_payload(limit * 3))
	return response

var/lastTgsError
var/suppress_bridge_spam = FALSE

/proc/TgsInfo(message)
	if(suppress_bridge_spam && findtext(message, "Export: http://127.0.0.1:") != 0)
		return
	world.log << "Info: [message]"

/proc/TgsError(message)
	lastTgsError = message
	if(suppress_bridge_spam && findtext(message, "Failed bridge request: http://127.0.0.1:") != 0)
		return
	world.log << "Err: [message]"

/proc/create_payload(size)
	var/builder = list()
	for(var/j = 0; j < size; ++j)
		builder += "a"
	var/payload = jointext(builder, "")
	return payload

/proc/CheckBridgeLimits()
	set waitfor = FALSE
	CheckBridgeLimitsImpl()


/proc/BridgeWithoutChunking(command, list/data)
	var/datum/tgs_api/v5/api = TGS_READ_GLOBAL(tgs)
	var/bridge_request = api.CreateBridgeRequest(command, data)
	suppress_bridge_spam = TRUE
	. = api.PerformBridgeRequest(bridge_request)
	suppress_bridge_spam = FALSE

/proc/CheckBridgeLimitsImpl()
	sleep(30)

	// Evil custom bridge command hacking here
	var/datum/tgs_api/v5/api = TGS_READ_GLOBAL(tgs)
	var/old_ai = api.access_identifier
	api.access_identifier = "tgs_integration_test"

	// Always send chat messages because they can have extremely large payloads with the text

	// bisecting request test
	var/base = 1
	var/nextPow = 0
	var/lastI = 0
	var/i
	lastTgsError = null
	for(i = 1; ; i = base + (2 ** nextPow))
		var/payload = create_payload(i)

		var/list/result = BridgeWithoutChunking(0, list("chatMessage" = list("text" = "payload:[payload]")))

		if(!result || lastTgsError || result["integrationHack"] != "ok")
			lastTgsError = null
			if(i == lastI + 1)
				break
			i = lastI
			base = lastI
			nextPow = 0
			continue

		lastI = i
		++nextPow

	// DMAPI5_BRIDGE_REQUEST_LIMIT
	var/limit = 8198

	// this actually gets doubled because it's in two fields for backwards compatibility, but that's fine
	var/list/final_result = api.Bridge(0, list("chatMessage" = list("text" = "done:[create_payload(limit * 3)]")))
	if(!final_result || lastTgsError || final_result["integrationHack"] != "ok")
		FailTest("Failed to end bridge limit test! [(istype(final_result) ? json_encode(final_result): (final_result || "null"))]")

	api.access_identifier = old_ai
