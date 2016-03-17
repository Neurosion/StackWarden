# StackWarden
Application stack resource monitors and dashboard.

### Monitors
- Monitor a resource and report its state to the configured handlers.
- Tags are currently not used, but are optional
- Display name is optional for all configuration

##### Http Availability 
- Attempts to connect to an http url
- Configuration Example
<pre>
{
	"Type": "Http.Availability",
	"Interval": 60000,
	"Address": "http://your.domain.here",
	"DisplayName": "Friendly Name Here",
	"Tags":[
		"Tag1",
		"Tag2"
	],
	"Handlers":[
		"HandlerName",
		"SomeOtherHandlerName"
	]
}
</pre>

##### SQL Database Presence
- Connects to a database server and queries that the database exists.
- Configuration Example
<pre>
{
	"Type": "Database.SQLPresence",
	"Interval": 60000,
	"ConnectionString": "connection string goes here",
	"DisplayName": "Friendly Name Here",
	"Tags":[
		"Tag1",
		"Tag2"
	],
	"Handlers":[
		"HandlerName",
		"SomeOtherHandlerName"
	]
}
</pre>

##### Machine Availability 
- Pings the target machine.
- Configuration Example
<pre>
{
	"Type": "Machine.Availability",
	"Interval": 60000,
	"Address": "MachineName",
	"DisplayName": "Friendly Name Here",
	"Tags":[
		"Tag1",
		"Tag2"
	],
	"Handlers":[
		"HandlerName",
		"SomeOtherHandlerName"
	]
}
</pre>
- Optional Configuration
  - WarningThreshold: A ping equal to or higher than this will trigger a warning state.
    - <tt>"WarningThreshold": 50</tt>
  - ErrorThreshold: A ping equal to or higher than this will trigger an error state.
    - <tt>"ErrorThreshold": 50</tt>

##### MSMQ Message Queue
- Checks queue size and aggregates message count per message type.
- Configuration Example
<pre>
{
	"Type": "MSMQ.QueueSize",
	"Interval": 60000,
	"Path": "FormatName:Direct=OS:server\\private$\\queue.name",
	"DisplayName": "Friendly Name Here",
	"Tags":[
		"Tag1",
		"Tag2"
	],
	"Handlers":[
		"HandlerName",
		"SomeOtherHandlerName"
	]
}
</pre>
- Optional Configuration
  - WarningThreshold: A ping equal to or higher than this will trigger a warning state.
    - <tt>"WarningThreshold": 50</tt>
  - ErrorThreshold: A ping equal to or higher than this will trigger an error state.
    - <tt>"ErrorThreshold": 50</tt>

##### Windows Service State
- Checks a machine for a windows service and its state.
- Configuration Example
<pre>
{
	"Type": "Service.State",
	"Interval": 60000,
	"Machine": "ServiceHostMachine",
	"Service": "Service Name",
	"DisplayName": "Friendly Name Here",
	"Tags":[
		"Tag1",
		"Tag2"
	],
	"Handlers":[
		"HandlerName",
		"SomeOtherHandlerName"
	]
}
</pre>

### Monitor Result Handlers
- Take results reported by monitors and forward them to the notification method and target they represent.
- The configuration files are named "YourHandlerNameHere.handlerconfig"
- The name used in the file name, before the extension, is used in the monitor configuration files to reference that handler.

##### Console
- For debugging

##### Dashboard
- Sends results to the StackWarden dashboard.
- Configuration Example
<pre>
{
	"Type": "Dashboard",
	"HookAddress": "http://your.domain.here/api/monitor/result/hook"
}
</pre>

##### Email
- Emails results to the specified recipients.
- SMTP settings are configured via the service's config file, for now.
- Configuration Example
<pre>
{
	"Type": "Email",
	"Sender": "sender@domain.com",
	"Recipients":[
		"recipient@one.com",
		"recipient@two.com"
	]
}
</pre>

##### Slack
- Sends results to the specific stack instance/channel.
- Configuration Example
<pre>
{
	"Type": "Slack",
	"HookAddress": "http://your.slack.hook/here"
}
</pre>
- Optional Configuration
  - Username: The username the results will be reported as coming from.
    - <tt>"Username": "UsernameHere"</tt>
  - Channel: The channel the results will be reported to.
    - <tt>"Channel": "ChannelNameHere"</tt>
  - Icon: The icon the user reporting the result will use.
    - <tt>"Icon": "http://icon.path.goes/here"</tt>
  - NotificationThreshold: The severity threshold used to determine if a message is sent to slack.
    - Values
	  - Normal
	  - Warning
	  - Error
	- <tt>"NotificationThreshold": "Warning"</tt>
