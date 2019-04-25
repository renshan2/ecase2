
	    // Load Scripts for document.ready  
	    _spBodyOnLoadFunctionNames.push('eCaseReadyFunction', 'BuildCalendar');

		function eCaseReadyFunction() {
			// Fix display issue with empty list tables
			if ($('table.ms-summarystandardbody > tbody > tr > td.ms-vb:contains("There are no items")').length) {
				$('td.ms-vb:contains("There are no items")').closest('table.ms-summarystandardbody').css('table-layout', 'auto');
			}
			// Get Unique Case ID from Details tab and copy to Case ID label
			if ($('#spanUniqueCaseID').length) {
				var txtCaseUniqID = $('#spanUniqueCaseID').text();
				if ($('#ctl00_PlaceHolderMain_lblCaseID').length) {
					$('#ctl00_PlaceHolderMain_lblCaseID').text(txtCaseUniqID);
				}
			}
			// Hide the Dates & Tasks list initially
			$('#divDatesTasksList').hide();
			// Toggle the visibility of the Dates & Tasks list  
			$('#lnkViewList').click(function () {
				$('#divDatesTasksList').toggle();
				if ($(this).html() === 'Hide List') {
					$(this).html('View List');
				} else {
					$(this).html('Hide List');
				}
				return false;
			});
			// Hide edit buttons for users without permission
			$().SPServices({
				operation: "GetRolesAndPermissionsForCurrentUser",
				async: false,
				completefunc: function (xData, Status) {
					//var userPerm = $(xData.responseXML).find("[nodeName=Permissions]").attr("Value");
					var userPerm = $(xData.responseXML).SPFilterNode("Permissions").attr("Value");
					var nonAdminP = userPerm == 1856436900591; // Contribute permission
					var adminP = userPerm == 9223372036854775807;
					var hideEdit = !(nonAdminP | adminP);
					if (hideEdit) {
						$('#lnkEditCase').hide();
					}
				}
			});            
			// Truncate note text 
			var divht = 28;
			var thisht = 0;
			$('.note-truncated a').each(function () {
				var noteA = $(this);
				thisht = $(noteA).outerHeight();
				if (thisht > 26) {
					// Note text is greater than two lines, truncate with ellipsis
					$(noteA).parent().dotdotdot({
						wrap: 'letter'
					});
				} else if (thisht < 14) {
					// Note text is only one line, reduce height of container
					$(noteA).parent().css("height", "15px");
				}
			});
		};
		// Function to delete list items
		function DeleteItem(itemID, listName) {
			try {
				var cnf = confirm("Are you sure you want to send the item(s) to the site Recycle Bin?");
				if (cnf) {
					var batchCmd = "<Batch OnError='Continue'><Method ID='1' Cmd='Delete'><Field Name='ID'>" + itemID + "</Field></Method></Batch>";
					// Use SPServices to delete the file.
					$().SPServices({
						operation: "UpdateListItems",
						async: false,
						listName: listName,
						updates: batchCmd,
						completefunc: function (xData, Status) {
							// Check the error codes for the web service call.
							$(xData.responseXML).SPFilterNode('ErrorCode').each(function () {
								responseError = $(this).text();
								if (responseError === '0x00000000') {
									window.location = window.location;
								}
								else {
									alert("There was an error trying to delete the item.");
								}
							});

						}
					});
				}
			} catch (ex) { alert(ex); }
		};
		// Function to remove note from displaying in right column
		function RemoveNote(itemID, listName) {
			try {
				var cnf = confirm("Are you sure you want to remove this note?");
				if (cnf) {
					// Use SPServices to update the list item
					$().SPServices({
						operation: "UpdateListItems",
						async: false,
						batchCmd: "Update",
						listName: listName,
						ID: itemID,
						valuepairs: [["Visible", 0]],
						completefunc: function (xData, Status) {
							// Check the error codes for the web service call.
							$(xData.responseXML).SPFilterNode('ErrorCode').each(function () {
								responseError = $(this).text();
								if (responseError === '0x00000000') {
									window.location = window.location;
								}
								else {
									alert("There was an error trying to update the note.");
								}
							});
						}
					});
				}
			} catch (ex) { alert(ex); }
		};

		// Calendar Scripts //
		//******************//

		// Format UTC dates as local date/time strings.
		function formatDateToLocal(date) {

			var dateUTC;

			if (typeof date === "string") {

				// Convert UTC string to date object
				var d = new Date();
				var year = date.split('-')[0];
				var month = date.split('-')[1] - 1;
				var day;
				var hour;
				var minute;
				var second;
				day = date.split('-')[2].split('T')[0];
				hour = date.split('T')[1].split(':')[0];
				minute = date.split('T')[1].split(':')[1].split(':')[0];
				second = date.split('T')[1].split(':')[2].split('Z')[0];
				dateUTC = new Date(Date.UTC(year, month, day, hour, minute, second));
			}
			else if (typeof date === "object") {
				dateUTC = date;
			}
			else {
				alert("Date is not a valid string or date object.");
			}

			// Create local date strings from UTC date object
			var year = "" + dateUTC.getFullYear();
			var month = "" + (dateUTC.getMonth() + 1); // Add 1 to month because months are zero-indexed.
			var day = "" + dateUTC.getDate();
			var hour = "" + dateUTC.getHours();
			var minute = "" + dateUTC.getMinutes();
			var second = "" + dateUTC.getSeconds();

			// Add leading zeros to single-digit months, days, hours, minutes, and seconds
			if (month.length < 2) {
				month = "0" + month;
			}
			if (day.length < 2) {
				day = "0" + day;
			}
			if (hour.length < 2) {
				hour = "0" + hour;
			}
			if (minute.length < 2) {
				minute = "0" + minute;
			}
			if (second.length < 2) {
				second = "0" + second;
			}

			var localDateString = year + "-" + month + "-" + day + "T" + hour + ":" + minute + ":" + second;

			return localDateString;
		}

		function BuildCalendar () {

		    $('#calendar').fullCalendar({
		        // Set calendar base options
		        header: {
		            left: 'prev,next today',
		            center: 'title',
		            right: 'month, agendaWeek, agendaDay'
		        },
		        defaultView: "month", 
		        firstHour: "5", 
		        //height: 720, 
		        contentHeight: 550,
		        weekMode: "liquid", 
		        theme: false, 
		        editable: false,
		        // Set "loading" image while waiting for calendar to load
		        loading: function (bool) {
		            if (bool)
		                $('#loading').show();
		            else
		                $('#loading').hide();
		        },

		        // Add events to the calendar
		        events: function (start, end, callback) {

		            // Create an array to hold the events
		            var events = [];

		            // Set the date from which to pull events based on the first visible day in the current calendar view
		            var startDate = $.fullCalendar.formatDate($('#calendar').fullCalendar('getView').start, "u").split("T")[0];
		            var startDateTasks = $.fullCalendar.formatDate($('#calendar').fullCalendar('getView').visStart, "u").split("T")[0];
		            var endDateTasks = $.fullCalendar.formatDate($('#calendar').fullCalendar('getView').visEnd, "u").split("T")[0];

		            // Get the current view of the calendar (agendaWeek, agendaDay, month, etc.), then set the camlView to this to limit result set 
		            var calView = $('#calendar').fullCalendar('getView').title;
		            var camlView = "";

		            switch (calView) {
		                case "agendaWeek":
		                    camlView = "<Week />";
		                    break;
		                case "agendaDay":
		                    camlView = "<Week />";
		                    break;
		                default: // Default to month view
		                    camlView = "<Month />";
		            }

		            // For calendar events, set the camlFields, camlQuery, and camlOptions to the appropriate values 
		            var camlFields = "<ViewFields><FieldRef Name='Title' /><FieldRef Name='EventDate' /><FieldRef Name='EndDate' /><FieldRef Name='Location' /><FieldRef Name='Description' /><FieldRef Name='FileDirRef' /><FieldRef Name='fRecurrence' /><FieldRef Name='RecurrenceData' /><FieldRef Name='RecurrenceID' /><FieldRef Name='fAllDayEvent' /></ViewFields>";
		            var camlQuery = "<Query><CalendarDate>" + startDate + "</CalendarDate><Where><DateRangesOverlap><FieldRef Name='EventDate' /><FieldRef Name='EndDate' /><FieldRef Name='RecurrenceID' /><Value Type='DateTime'>" + camlView + "</Value></DateRangesOverlap></Where><OrderBy><FieldRef Name='EventDate' /></OrderBy></Query>";
		            var camlOptions = "<QueryOptions><CalendarDate>" + startDate + "</CalendarDate><RecurrencePatternXMLVersion>v3</RecurrencePatternXMLVersion><ExpandRecurrence>TRUE</ExpandRecurrence><DateInUtc>TRUE</DateInUtc></QueryOptions>";

		            // For tasks, set the camlFields, camlQuery, and camlOptions to the appropriate values
		            var camlFieldsTasks = "<ViewFields><FieldRef Name='Title' /><FieldRef Name='StartDate' /><FieldRef Name='DueDate' /><FieldRef Name='Body' /><FieldRef Name='FileDirRef' /></ViewFields>";
		            // var camlQueryTasks = "<Query><Where><And><Leq><FieldRef Name='StartDate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>" + endDateTasks + "</Value></Leq><Geq><FieldRef Name='DueDate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>" + startDateTasks + "</Value></Geq></And></Where></Query>";
		            // More complex query to include tasks without a StartDate
		            var camlQueryTasks = "<Query><Where><Or><And><Leq><FieldRef Name='StartDate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>" + endDateTasks + "</Value></Leq><Geq><FieldRef Name='DueDate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>" + startDateTasks + "</Value></Geq></And><And><IsNull><FieldRef Name='StartDate' /></IsNull><And><Leq><FieldRef Name='DueDate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>" + endDateTasks + "</Value></Leq><Geq><FieldRef Name='DueDate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>" + startDateTasks + "</Value></Geq></And></And></Or></Where></Query>";

		            // Make the web service call to retrieve events
		            $().SPServices({
		                operation: "GetListItems",
		                async: false,
		                listName: "Case Related Dates", 
		                CAMLViewFields: camlFields,
		                CAMLQuery: camlQuery,
		                CAMLQueryOptions: camlOptions,
		                completefunc: function (xData, Status) {
		                    $(xData.responseXML).SPFilterNode('z:row').each(function () {

		                        // Check for all day events
		                        var fADE = $(this).attr('ows_fAllDayEvent');
		                        var thisADE = false;
		                        var thisStart;
		                        var thisEnd;
		                        if (typeof fADE !== "undefined" && fADE !== "0") {
		                            thisADE = true;
		                            // Get the start and end date/time of the all day event
		                            var thisStart = $(this).attr('ows_EventDate');
		                            var thisEnd = $(this).attr('ows_EndDate');
		                        }
		                        else {
		                            // Get the start and end date/time of the event
		                            var thisStart = formatDateToLocal($(this).attr('ows_EventDate'));
		                            var thisEnd = formatDateToLocal($(this).attr('ows_EndDate'));
		                        }

		                        // Get the list item ID and recurrence date if present
		                        var thisID = $(this).attr('ows_ID').split(';#').join('.');

		                        // FullCalendar documentation specifies that recurring events should all have the same id value when building the events array 
		                        var eventID = thisID.split('.')[0];

		                        // Get the event title
		                        var thisTitle = 'Event: ' + $(this).attr('ows_Title');

		                        // Get the event description
		                        var thisDesc = $(this).attr('ows_Description');

		                        // Get the item url
		                        var urlValue = $(this).attr('ows_FileDirRef');
		                        var urlArray;
		                        var urlName;
		                        if (urlValue == undefined) {
		                            urlName = "";
		                        }
		                        else {
		                            urlArray = urlValue.split(";#");
		                            urlName = urlArray[1];
		                        }

		                        // Add the event information to the events array
		                        events.push({
		                            title: thisTitle,
		                            id: eventID,
		                            start: thisStart,
		                            end: thisEnd,
		                            allDay: thisADE,                                                                         
		                            url: '/' + urlName + '/Dispform.aspx?ID=' + thisID + '&Source=' + window.location, // URL to link to the item display form
		                            description: thisDesc
		                        });

		                    });

		                    // callback(events); // Wait to call this until after tasks are added                            

		                    // Make the web service call to retrieve tasks
		                    $().SPServices({
		                        operation: "GetListItems",
		                        async: false,
		                        listName: "Activities &amp; Tasks", 
		                        CAMLViewFields: camlFieldsTasks,
		                        CAMLQuery: camlQueryTasks,
		                        completefunc: function (xData, Status) {
		                            $(xData.responseXML).SPFilterNode('z:row').each(function () {

		                                // Set tasks as all day events
		                                var thisADE = true;
		                                var thisStart;
		                                var thisEnd;

		                                // Get the start and end date of the task 
		                                var thisStart = $(this).attr('ows_StartDate');
		                                var thisEnd = $(this).attr('ows_DueDate');
		                                // If StartDate is null, set start date to the same as DueDate
		                                if (thisStart == undefined) {
		                                    thisStart = thisEnd;
		                                }

		                                // Get the list item ID 
		                                var thisID = $(this).attr('ows_ID');

		                                // Set FullCalendar event ID
		                                var eventID = thisID;

		                                // Get the task title
		                                var thisTitle = 'Task: ' + $(this).attr('ows_Title');

		                                // Get the task description
		                                var thisDesc = $(this).attr('ows_Body');

		                                // Get the item url
		                                var urlValue = $(this).attr('ows_FileDirRef');
		                                var urlArray;
		                                var urlName;
		                                if (urlValue == undefined) {
		                                    urlName = "";
		                                }
		                                else {
		                                    urlArray = urlValue.split(";#");
		                                    urlName = urlArray[1];
		                                }

		                                // Add the task information to the events array 
		                                events.push({
		                                    title: thisTitle,
		                                    id: eventID,
		                                    start: thisStart,
		                                    end: thisEnd,
		                                    allDay: thisADE,
		                                    url: '/' + urlName + '/Dispform.aspx?ID=' + thisID + '&Source=' + window.location, // URL to link to the item display form
		                                    description: thisDesc
		                                });

		                            });

		                            // Callback FullCalendar with events array
		                            callback(events);
		                        }
		                    });

		                }
		            });
										
		        },
		        eventClick: function (event) {
		            if (event.url) {
		                ShowPopupDialog(event.url); // Change event link to dialog
		                return false;
		            }
		        }
		    });

		};

