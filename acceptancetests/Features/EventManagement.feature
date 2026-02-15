Feature: Event Management
  Event lifecycle management including creation and visibility

@Define_Event
Scenario: Authenticated user creates an event successfully
  Given a registered user named 'Ali' exists
  When Ali creates an event with the following details
    | Name        | Capacity | StartDate       | EndDate         |
    | Tech Event  | 100      | 2025-05-01 10:00| 2025-05-01 18:00|
  And He's event will be created in the Draft Status

@Seeing_Event
Scenario: User can see only published events
  Given the following events exist in the system
    | Name    | Status    |
    | Event 1 | Draft     |
    | Event 2 | Published |
    | Event 3 | Cancelled  |
    | Event 4 | Published |
  When a user requests the public event list
  Then the user should see exactly the following events
    | Name    |
    | Event 2 |
    | Event 4 |
