# TPRequest
Used for survival terraria servers, request/accept/reject players using available commands. Allow to teleport to someone's location by requesting. You can also accept/reject someone's request.

# How To Install
1. Download the .dll file.
2. Put the .dll file inside of /ServerPlugins/
3. Stop and rerun the server.

# Versions
TPRequest v1.0.0

# Instructions
## Commands
`/tpa <player>` Request a player to teleport you to that player's location
`/tpaccept` Accept the current requesting player (usable when you are requested)
`/tpreject` Reject the current requesting player (usable when you are requested)

## Permissions
### How to apply permissions
1. Check the groupNames (optional) `/group list`
2. Add the permission `/group addperm <groupName> <perm>`

### Permissions List
`tpa.request` for `/tpa` 
`tpa.accept` for `/tpaccept`
`tpa.reject` for `/tpreject`
