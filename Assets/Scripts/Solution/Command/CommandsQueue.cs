namespace Solution.Command
{
    using System;
    using System.Collections.Generic;

    public class CommandsQueue
    {
        private readonly int _capacity;
        private readonly List<ICommandBase> _items;
        private int _currentIndex;

        public CommandsQueue(int capacity)
        {
            _capacity = capacity;
            _items = new List<ICommandBase>();
            _currentIndex = -1;
        }

        public void Add(ICommandBase item)
        {
            if (_items.Count == _capacity)
            {
                // Remove the oldest item (the first one in the list)
                _items.RemoveAt(0);
            }
            else if (_currentIndex < _items.Count - 1)
            {
                // If we are in the process of undoing, remove elements after the current index
                _items.RemoveRange(_currentIndex + 1, _items.Count - _currentIndex - 1);
            }
            
            _items.Add(item);
            _currentIndex = _items.Count - 1;
        }

        public bool Undo()
        {
            if (_currentIndex >= 0)
            {
                CurrentCommand.Undo();
                _currentIndex--;
                return true;
            }
            return false;
        }
        
        public bool Redo()
        {
            if (_currentIndex < _items.Count - 1)
            {
                _currentIndex++;
                CurrentCommand.Redo();
                return true;
            }
            return false; 
        }

        private ICommandBase CurrentCommand
        {
            get
            {
                if (_currentIndex >= 0 && _currentIndex < _items.Count)
                {
                    return _items[_currentIndex];
                }
                throw new InvalidOperationException("The queue is empty.");
            }
        }
    }
    
    public static class CommandQueueExtensionsForCommands
    {
        public static T AddToQueue<T>(this T command, CommandsQueue commandsQueue) where T: class, ICommandBase
        {
            commandsQueue.Add(command);
            return command;
        }
    }
}