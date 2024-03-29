﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace FormsLib.Misc
{
    /// <summary>
    /// Provides a thread-safe binding-list by using the synchronizationcontext where the list was created.
    /// HACK: Probably not full-proof.  
    /// Copied from: http://groups.google.co.uk/group/microsoft.public.dotnet.languages.csharp/msg/f12a3c5980567f06
    /// List should be created in a forms control providing a synchronization context
    /// More about synchronizationcontext here: http://www.codeproject.com/KB/cpp/SyncContextTutorial.aspx
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ThreadsafeBindingList<T> : BindingList<T>
    {
        private readonly SynchronizationContext synchronizationContext;

        protected override void OnAddingNew(AddingNewEventArgs e)
        {
            synchronizationContext.Send(delegate { BaseAddingNew(e); }, null);
        }


        void BaseAddingNew(AddingNewEventArgs e)
        {
            base.OnAddingNew(e);
        }

        protected override void OnListChanged(ListChangedEventArgs e)
        {
            synchronizationContext.Send(
                delegate { BaseListChanged(e); }, null);
        }

        void BaseListChanged(ListChangedEventArgs e)
        {
            base.OnListChanged(e);
        }

        /// <summary>
        /// Creates a bindinglist using the givin synchronization context
        /// </summary>
        public ThreadsafeBindingList(SynchronizationContext context)
        {
            synchronizationContext = context;
            //for now this class can only be used in synchronization context
            if (synchronizationContext == null)
                throw new InvalidOperationException("Unable to create a threadsafe bindinglist without a synchronization context");
        }

        public ThreadsafeBindingList(SynchronizationContext context, IList<T> list)
            : base(list)
        {
            synchronizationContext = context;
            //for now this class can only be used in synchronization contexts
            if (synchronizationContext == null)
                throw new InvalidOperationException("Unable to create a threadsafe bindinglist without a synchronization context");
        }
    }
}