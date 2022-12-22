using FrooxEngine;

namespace JworkzNeosMod.Extensions
{
    internal static class UserExtensions
    {
        public static bool IsAllocatingUserOf(this User user, IWorldElement worldEl)
        {
            ulong position;
            byte userInBytes;
            worldEl.ReferenceID.ExtractIDs(out position, out userInBytes);
            User userByAllocationId = worldEl.World.GetUserByAllocationID(userInBytes);
            return user != null && userByAllocationId?.UserID == user.UserID && position >= userByAllocationId.AllocationIDStart;
        }
    }
}
